using Microsoft.AspNetCore.Identity;
using SmartClinicQueue.Application.DTO_s;
using SmartClinicQueue.Application.Interfaces.IServices;
using SmartClinicQueue.Domain.Constant;
using SmartClinicQueue.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartClinicQueue.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtService _jwtService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtService jwtService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            if (existingUser is not null)
                throw new InvalidOperationException(
                    "Email is already registered.");

            var user = new ApplicationUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(
                user,
                dto.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    string.Join(" | ",
                        result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(
                user,
                Roles.Patient);

            var token = await _jwtService.CreateTokenAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = Roles.Patient
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                dto.Password,
                lockoutOnFailure: true);

            if (result.IsLockedOut)
                throw new UnauthorizedAccessException(
                    "Account is temporarily locked.");

            if (!result.Succeeded)
                throw new UnauthorizedAccessException(
                    "Invalid email or password.");

            var roles = await _userManager.GetRolesAsync(user);

            var token = await _jwtService.CreateTokenAsync(user);

            return new AuthResponseDto
            {
                Token = token,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = roles.FirstOrDefault() ?? Roles.Patient
            };
        }
    }
}
