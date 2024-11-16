using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NutritionTargetsController : ControllerBase
    {
        private INutritionTargetService _nutritionService;
        private UserManager<FitnessUser> _userManager;

        public NutritionTargetsController(
            INutritionTargetService nutritionService,
            UserManager<FitnessUser> userManager)
        {
            _nutritionService = nutritionService;
            _userManager = userManager;
        }

        // GET: api/NutritionTargets/5
        [HttpGet]
        public async Task<ActionResult<NutritionTarget>> GetNutritionTargets()
        {
            var userId = await GetUserIdAsync();
            return await _nutritionService.GetNutritionTarget(userId);
        }

        // POST: api/NutritionTargets
        [HttpPost]
        public async Task<ActionResult<NutritionTarget>> PostNutritionTargets(NutritionTargetDTO nutriotionTarget)
        {
            var userId = await GetUserIdAsync();
            var entity = await _nutritionService.AddNutritionTarget(userId, nutriotionTarget);

            if (entity == null)
            {
                return NotFound($"NutritionTarget with user Id = {userId} was not added");
            }

            return CreatedAtAction("GetNutritionTargets", new { id = entity.Id }, nutriotionTarget);
        }

        // PUT: api/NutritionTarget/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNutritionTargets(int id, NutritionTargetDTO nutriotionTarget)
        {
            var userId = await GetUserIdAsync();
            await _nutritionService.UpdateNutritionTarget(id, userId, nutriotionTarget);
            return NoContent();
        }

        // DELETE: api/NutritionTarget/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNutritionTargets(int id)
        {
            var userId = await GetUserIdAsync();
            await _nutritionService.DeleteNutritionTarget(id, userId);

            return NoContent();
        }

        private async Task<int> GetUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return user.Id;
        }
    }
}
