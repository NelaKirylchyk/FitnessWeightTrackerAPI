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
        private readonly UserManager<MyIdentityUser> _userManager;
        private readonly SignInManager<MyIdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<MyIdentityUser> userManager,
            SignInManager<MyIdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpGet("signin-google")]
        public IActionResult SignInWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            if (result?.Principal == null)
            {
                return Redirect("~/");
            }

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new MyIdentityUser { UserName = email, Email = email };
                var res = await _userManager.CreateAsync(user);

                if (!res.Succeeded)
                {
                    return BadRequest(res.Errors);
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            var token = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "jwt");

            return Ok(new { Token = token });
        }
    }
}
