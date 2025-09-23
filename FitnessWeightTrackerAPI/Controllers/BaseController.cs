using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public abstract class BaseController : ControllerBase
{
    private readonly UserManager<FitnessUser> _userManager;

    protected BaseController(UserManager<FitnessUser> userManager)
    {
        _userManager = userManager;
    }

    protected int GetUserIdAsync()
    {
        var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                      ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID claim not found.");

        if (!int.TryParse(userIdClaim, out var userId))
            throw new ArgumentException($"User ID claim '{userIdClaim}' is not a valid int.");

        return userId;
    }

}