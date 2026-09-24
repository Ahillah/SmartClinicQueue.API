using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartClinicQueue.Application.Common;
using SmartClinicQueue.Application.DTO_s;
using SmartClinicQueue.Application.Interfaces.IServices;

namespace SmartClinicQueue.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            return Ok(ApiResponse<AuthResponseDto>.Success(result));
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            return Ok(ApiResponse<AuthResponseDto>.Success(result));
        }
    }
}
