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

    protected async Task<int> GetUserIdAsync()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        return user.Id;
    }
}