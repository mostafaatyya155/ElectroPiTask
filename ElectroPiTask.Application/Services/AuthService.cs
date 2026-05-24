using ElectroPiTask.Application.Common.Exceptions;
using ElectroPiTask.Application.Common.Interfaces;
using ElectroPiTask.Application.DTOs.Auth;
using ElectroPiTask.Domain.Entities;
using ElectroPiTask.Domain.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectroPiTask.Application.Services
{
    public class AuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly ILogger<AuthService> _logger;
        private readonly TokenService _tokenService;

        public AuthService(
            IRepository<User> userRepository,
            ILogger<AuthService> logger,
            TokenService tokenService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterDto dto)
        {
            _logger.LogDebug("Starting user registration for {Email}", dto.Email);

            // Check if email exists
            if (await _userRepository.ExistsAsync(u => u.Email == dto.Email))
            {
                _logger.LogWarning("Email already exists: {Email}", dto.Email);
                throw new BusinessException("Email already exists");
            }

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = dto.Role
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            _logger.LogDebug("User created successfully with ID: {UserId}", user.Id);

            return new AuthResponseDto
            {
                Token = _tokenService.GenerateJwtToken(user),
                Email = user.Email,
                FullName = user.FullName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            _logger.LogDebug("Starting login process for {Email}", dto.Email);

            var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Failed login attempt for {Email} f", dto.Email);
                throw new BusinessException("Invalid email or password");
            }

            _logger.LogDebug("User {Email} logged in successfully", dto.Email);

            return new AuthResponseDto
            {
                Token = _tokenService.GenerateJwtToken(user),
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Role = user.Role.ToString(),
            };
        }
    }
}
