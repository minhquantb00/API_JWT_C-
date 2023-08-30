using API_JWT_C_.Payloads.DTOs;
using API_JWT_C_.Payloads.Requests;
using API_JWT_C_.Service.IService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API_JWT_C_.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;
        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }
        [HttpPost]
        [Route("/api/auth/register")]
        public async Task<IActionResult> Register([FromForm]RegisterRequest register)
        {
            var result = await _authService.RegisterRequest(register);
            if (result == null)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost]
        [Route(("/api/auth/login"))]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.Login(request);
            if (result == null)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("/api/auth/renew-token")]
        public IActionResult RenewToken(TokenDTO token)
        {
            var result = _authService.RenewAccessToken(token);
            if(result == null)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("/api/auth/get-all")]
        [Authorize(Roles = "ADMIN, MOD")]
        public async Task<IActionResult> GetAlls(int pageSize, int pageNumber)
        {
            return Ok(await _authService.GetAlls(pageSize, pageNumber));
        }
        [HttpPut]
        [Route("/api/auth/change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword(Request_ChangePassword request)
        {
            int id = Convert.ToInt32(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _authService.ChangePassword(id, request));

        }
        [HttpPost]
        [Route("/api/auth/forgot-password")]
        public async Task<IActionResult> ForgotPassword(Request_ForgotPassword request)
        {
            return Ok(await _authService.ForgotPassword(request));
        }

        [HttpPost]
        [Route("/api/auth/create-new-password")]
        public async Task<IActionResult> CreateNewPassword(ConfirmCreateNewPassword request)
        {
            return Ok(await _authService.CreateNewPassword(request));
        }
    }
}
