using ElectroPiTask.Application.Common.Models;
using ElectroPiTask.Application.DTOs.Auth;
using ElectroPiTask.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectroPiTask.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register(RegisterDto dto)
        {
            _logger.LogInformation("Registering user with email: {Email}", dto.Email);
            var result = await _authService.RegisterUserAsync(dto);
            _logger.LogInformation("User registered successfully with email: {Email}", result.Email);

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(
                result,
                "User registered successfully."));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto dto)
        {
            _logger.LogInformation("Login attempt for email: {Email}", dto.Email);
            var result = await _authService.LoginAsync(dto);
            _logger.LogInformation("Login successful for email: {Email}", dto.Email);

            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
        }

    }
}
