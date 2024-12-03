using System.Security.Claims;
using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<FitnessUser> _userManager;
        private readonly SignInManager<FitnessUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<FitnessUser> userManager,
            SignInManager<FitnessUser> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("signin-google")]
        public IActionResult SignInWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!result.Succeeded)
            {
                _logger.LogInformation("Authentication was not successful.");
                return TypedResults.Forbid();
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new FitnessUser
                {
                    UserName = email,
                    Email = email
                };
                var res = await _userManager.CreateAsync(user);

                if (!res.Succeeded)
                {
                    _logger.LogInformation("A problem while registering a new user.");
                    return TypedResults.Problem(res.ToString(), statusCode: StatusCodes.Status401Unauthorized);
                }
            }

            _signInManager.AuthenticationScheme = IdentityConstants.BearerScheme;
            await _signInManager.SignInAsync(user, isPersistent: false);
            _logger.LogInformation("Authentication was successful.");

            return TypedResults.Empty;
        }
    }
}
