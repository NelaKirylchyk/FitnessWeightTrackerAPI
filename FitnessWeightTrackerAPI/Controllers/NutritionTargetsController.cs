using System.Security.Claims;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NutritionTargetsController : ControllerBase
    {
        private INutritionTargetService _nutritionService;

        public NutritionTargetsController(INutritionTargetService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        // GET: api/NutritionTargets/5
        [HttpGet]
        public async Task<ActionResult<NutritionTarget>> GetNutritionTargets()
        {
            var userId = GetUserIdFromClaim();
            return await _nutritionService.GetNutritionTarget(userId);
        }

        // POST: api/NutritionTargets
        [HttpPost]
        public async Task<ActionResult<NutritionTarget>> PostNutritionTargets(NutritionTargetDTO nutriotionTarget)
        {
            var userId = GetUserIdFromClaim();
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
            var userId = GetUserIdFromClaim();
            await _nutritionService.UpdateNutritionTarget(id, userId, nutriotionTarget);
            return NoContent();
        }

        // DELETE: api/NutritionTarget/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNutritionTargets(int id)
        {
            var userId = GetUserIdFromClaim();
            await _nutritionService.DeleteNutritionTarget(id, userId);

            return NoContent();
        }

        private int GetUserIdFromClaim()
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }
    }
}
