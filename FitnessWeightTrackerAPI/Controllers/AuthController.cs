using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using static System.Net.WebRequestMethods;

namespace FitnessWeightTrackerAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userRepository)
        {
            _configuration = configuration;
            _userService = userRepository;
        }

        [HttpGet("signin-google")]
        public IActionResult SignInWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth");
            var properties = new AuthenticationProperties { RedirectUri = $"https://localhost:7231{redirectUrl}" };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (result?.Principal == null)
            {
                return Redirect("~/");
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            User user = await _userService.GetUserByEmailAsync(email) ?? new User
            {
                UserName = email,
                Email = email,
                Name = result.Principal.FindFirstValue(ClaimTypes.GivenName) ?? email,
                Surname = result.Principal.FindFirstValue(ClaimTypes.Surname) ?? email
            };

            if (user.Id == 0)
            {
                await _userService.RegisterUserAsync(new RegistrationUserDTO()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname
                });
            }

            var jwtToken = _userService.GenerateUserJWTToken(user);
            return Ok(new { token = jwtToken });
        }
    }
}
