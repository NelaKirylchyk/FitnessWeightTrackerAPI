namespace FitnessWeightTrackerAPI.Controllers
{
    using System.Security.Claims;
    using FitnessWeightTrackerAPI.Data.DTO;
    using FitnessWeightTrackerAPI.Models;
    using FitnessWeightTrackerAPI.Services.Interfaces;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
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
                Surname = result.Principal.FindFirstValue(ClaimTypes.Surname) ?? email,
            };

            if (user.Id == 0)
            {
                var newUser = new RegistrationUserDTO()
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Name = user.Name,
                    Surname = user.Surname
                };

                await _userService.RegisterUserAsync(newUser, isExternalUser: true);
            }

            var jwtToken = _userService.GenerateUserJWTToken(user);
            return Ok(new { token = jwtToken });
        }
    }
}
