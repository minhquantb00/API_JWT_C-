using API_JWT_C_.Entities;
using API_JWT_C_.Handler.Email;
using API_JWT_C_.Handler.Image;
using API_JWT_C_.Payloads.Converters;
using API_JWT_C_.Payloads.DTOs;
using API_JWT_C_.Payloads.Requests;
using API_JWT_C_.Payloads.Responses;
using API_JWT_C_.Service.IService;
using AutoMapper;
using Azure.Core;
using CloudinaryDotNet;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using BCryptNet = BCrypt.Net.BCrypt;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace API_JWT_C_.Service.Implements
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ResponseObject<UserDTO> _responseObject;
        private readonly ResponseObject<TokenDTO> _responseObjectToken;
        private readonly UserConverter _userConverter;
        public AuthService(
            IMapper mapper,
            IConfiguration configuration,
            ResponseObject<UserDTO> responseObject,
            ResponseObject<TokenDTO> reponseObjectToken,
            UserConverter userConverter
            )
        {
            _mapper = mapper;
            _configuration = configuration;
            _responseObject = responseObject;
            _responseObjectToken = reponseObjectToken;
            _userConverter = userConverter;
        }

        public async Task<ResponseObject<UserDTO>> ChangePassword(int userId,Request_ChangePassword request)
        {
            var user = await _context.users.FirstOrDefaultAsync(x => x.Id == userId);
            if (!BCryptNet.Verify(request.OldPassword, user.Password))
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Mật khẩu cũ không chính xác", null);
            }
            user.Password = BCryptNet.HashPassword(request.NewPassword);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Đối mật khẩu thành công", _userConverter.EntityToDTO(user));
        }
        public string SendEmail(EmailTo emailTo)
        {
            if (!Validate.IsValidEmail(emailTo.Mail))
            {
                return "Định dạng email không hợp lệ";
            }
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("minhquantb00@gmail.com", "jvztzxbtyugsiaea"),
                EnableSsl = true
            };
            try
            {
                var message = new MailMessage();
                message.From = new MailAddress("minhquantb00@gmail.com");
                message.To.Add(emailTo.Mail);
                message.Subject = emailTo.Subject;
                message.Body = emailTo.Content;
                message.IsBodyHtml = true;
                smtpClient.Send(message);

                return "Gửi email thành công";
            }
            catch (Exception ex)
            {
                return "Lỗi khi gửi email: " + ex.Message;
            }
        }



        public TokenDTO GenerateAccessToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value!);

            var decentralization = _context.roles.FirstOrDefault(x => x.Id == user.RoleId);

            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("Username", user.UserName),
                    new Claim("Avatar", user.AvatarUrl),
                    new Claim("RoleId", user.RoleId.ToString()),
                    new Claim(ClaimTypes.Role, decentralization?.RoleName ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(4),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = jwtTokenHandler.CreateToken(tokenDescription);
            var accessToken = jwtTokenHandler.WriteToken(token);
            var refreshToken = GenerateRefreshToken();

            RefreshToken rf = new RefreshToken
            {
                Token = refreshToken,
                ExpiredTime = DateTime.UtcNow.AddHours(4),
                UserId = user.Id
            };

            _context.refreshTokens.Add(rf);
            _context.SaveChanges();

            TokenDTO tokenDTO = new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
            return tokenDTO;
        }

        public string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);
                return Convert.ToBase64String(random);
            }
        }

        public async Task<IEnumerable<UserDTO>> GetAlls(int pageSize, int pageNumber)
        {
            var list = await _context.users.Skip((pageNumber - 1) * pageSize).Take(pageSize).Select(x => _userConverter.EntityToDTO(x)).ToListAsync();
            return list;
        }

        public async Task<ResponseObject<TokenDTO>> Login(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.UserName))
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status400BadRequest, "Vui lòng điền đầy đủ thông tin", null);
            }

            var user = await _context.users.FirstOrDefaultAsync(x => x.UserName.Equals(request.UserName));
            if (user is null)
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status404NotFound, "Tên tài khoản không tồn tại", null);
            }

            bool isPasswordValid = BCryptNet.Verify(request.Password, user.Password);
            if (!isPasswordValid)
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status400BadRequest, "Tên đăng nhập hoặc mật khẩu không chính xác", null);
            }
            else
            {
                return _responseObjectToken.ResponseSuccess("Đăng nhập thành công", GenerateAccessToken(user));
            }
        }


        public async Task<ResponseObject<UserDTO>> RegisterRequest(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName)
               || string.IsNullOrWhiteSpace(request.Password)
               || string.IsNullOrWhiteSpace(request.FirstName)
               || string.IsNullOrWhiteSpace(request.LastName)
               || string.IsNullOrWhiteSpace(request.Email))
            {
                return _responseObject.ResponseError(StatusCodes.Status404NotFound, "Bạn cần truyền vào đầy đủ thông tin", null);
            }

            if (Validate.IsValidEmail(request.Email) == false)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Định dạng Email không hợp lệ", null);
            }

            if (await _context.users.FirstOrDefaultAsync(x => x.UserName.Equals(request.UserName)) != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Tên tài khoản đã tồn tại trên hệ thống", null);
            }
            if (await _context.users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email)) != null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Email đã tồn tại trên hệ thống", null);
            }
            else
            {
                int imageSize = 2 * 1024 * 768;
                try
                {
                    User user = new User();
                    user.UserName = request.UserName;
                    user.FirstName = request.FirstName;
                    user.Password = BCryptNet.HashPassword(request.Password);
                    user.LastName = request.LastName;
                    user.Email = request.Email;
                    user.DateOfBirth = request.DateOfBirth;
                    user.RoleId = 3;
                    string imageUrl = "";
                    if (request.AvatarUrl != null)
                    {
                        if (!HandleImage.IsImage(request.AvatarUrl, imageSize))
                        {
                            return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Ảnh không hợp lệ", null);
                        }
                        else
                        {
                            var avatarFile = await HandleUploadImage.Upfile(request.AvatarUrl);
                            user.AvatarUrl = avatarFile == "" ? "https://media.istockphoto.com/id/1300845620/vector/user-icon-flat-isolated-on-white-background-user-symbol-vector-illustration.jpg?s=612x612&w=0&k=20&c=yBeyba0hUkh14_jgv1OKqIH0CCSWU_4ckRkAoy2p73o=" : avatarFile;
                        }
                    }

                    await _context.users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    return _responseObject.ResponseSuccess("Đăng ký tài khoản thành công", _userConverter.EntityToDTO(user));
                } catch (Exception ex)
                {
                    return _responseObject.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
                }
            }
        }

        public ResponseObject<TokenDTO> RenewAccessToken(TokenDTO request)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value);

            var tokenValidation = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value))
            };

            try
            {
                var tokenAuthentication = jwtTokenHandler.ValidateToken(request.AccessToken, tokenValidation, out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtSecurityToken || jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status400BadRequest, "Token không hợp lệ", null);
                }
                RefreshToken refreshToken = _context.refreshTokens.FirstOrDefault(x => x.Token == request.RefreshToken);
                if (refreshToken == null)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status404NotFound, "RefreshToken không tồn tại trong database", null);
                }
                if (refreshToken.ExpiredTime < DateTime.Now)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status401Unauthorized, "Token chưa hết hạn", null);
                }
                var user = _context.users.FirstOrDefault(x => x.Id == refreshToken.UserId);
                if (user == null)
                {
                    return _responseObjectToken.ResponseError(StatusCodes.Status404NotFound, "Người dùng không tồn tại", null);
                }
                var newToken = GenerateAccessToken(user);

                return _responseObjectToken.ResponseSuccess("Làm mới token thành công", newToken);
            }
            catch (Exception ex)
            {
                return _responseObjectToken.ResponseError(StatusCodes.Status500InternalServerError, ex.Message, null);
            }
        }

        public async Task<string> ForgotPassword(Request_ForgotPassword request)
        {
            User user = await _context.users.FirstOrDefaultAsync(x => x.Email.Equals(request.Email));
            if (user is null)
            {
                return "Email không tồn tại trong hệ thống";
            }
            else
            {
                var confirms = _context.confirmEmails.Where(x => x.UserId == user.Id).ToList();
                _context.confirmEmails.RemoveRange(confirms);
                await _context.SaveChangesAsync();
                ConfirmEmail confirmEmail = new ConfirmEmail
                {
                    UserId = user.Id,
                    IsConfirm = false,
                    ExpiredTime = DateTime.Now.AddHours(4),
                    CodeActive = "MyBugs" + GenerateCodeActive().ToString()
                };
                await _context.confirmEmails.AddAsync(confirmEmail);
                await _context.SaveChangesAsync();
                string message =  SendEmail(new EmailTo
                {
                    Mail = request.Email,
                    Subject = "Nhận mã xác nhận để tạo mật khẩu mới từ đây: ",
                    Content = $"Mã kích hoạt của bạn là: {confirmEmail.CodeActive}, mã này sẽ hết hạn sau 4 tiếng"
                });
                return "Gửi mã xác nhận về email thành công, vui lòng kiểm tra email";
            }
        }
        public async Task<ResponseObject<UserDTO>> CreateNewPassword(ConfirmCreateNewPassword request)
        {
            ConfirmEmail confirmEmail = await _context.confirmEmails.Where(x => x.CodeActive.Equals(request.CodeActive)).FirstOrDefaultAsync();
            if(confirmEmail is null)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận không chính xác", null);
            }
            if(confirmEmail.ExpiredTime < DateTime.Now)
            {
                return _responseObject.ResponseError(StatusCodes.Status400BadRequest, "Mã xác nhận đã hết hạn", null);
            }
            User user = _context.users.FirstOrDefault(x => x.Id == confirmEmail.UserId);
            user.Password = BCryptNet.HashPassword(request.NewPassword);
            _context.confirmEmails.Remove(confirmEmail);
            _context.users.Update(user);
            await _context.SaveChangesAsync();
            return _responseObject.ResponseSuccess("Tạo mật khẩu mới thành công", _userConverter.EntityToDTO(user));

        }
        private int GenerateCodeActive()
        {
            Random random = new Random();
            return random.Next(100000, 999999);
        }
    }
}

