using API_JWT_C_.Entities;
using API_JWT_C_.Handler.Email;
using API_JWT_C_.Payloads.DTOs;
using API_JWT_C_.Payloads.Requests;
using API_JWT_C_.Payloads.Responses;

namespace API_JWT_C_.Service.IService
{
    public interface IAuthService
    {
        string GenerateRefreshToken();
        TokenDTO GenerateAccessToken(User user);
        ResponseObject<TokenDTO> RenewAccessToken(TokenDTO request);
        Task<ResponseObject<TokenDTO>> Login(LoginRequest request);
        Task<ResponseObject<UserDTO>> RegisterRequest(RegisterRequest request);
        Task<IEnumerable<UserDTO>> GetAlls(int pageSize, int pageNumber);
        Task<ResponseObject<UserDTO>> ChangePassword(int userId, Request_ChangePassword request);
        string SendEmail(EmailTo emailTo);
        Task<string> ForgotPassword(Request_ForgotPassword request);
        Task<ResponseObject<UserDTO>> CreateNewPassword(ConfirmCreateNewPassword request);
    }
}
