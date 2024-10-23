using Microsoft.AspNetCore.Mvc;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Services.Interfaces;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionTargetsController : ControllerBase
    {
        private INutritionService _nutritionService;

        public NutritionTargetsController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        // GET: api/NutritionTargets/5
        [HttpGet]
        public async Task<ActionResult<NutritionTarget>> GetNutritionTargets()
        {
            int userId = GetCurrentUserId();
            return await _nutritionService.GetNutritionTarget(userId);
        }

        // POST: api/NutritionTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NutritionTarget>> PostNutritionTargets(NutritionTargetDTO nutriotionTarget)
        {
            var userId = GetCurrentUserId();
            var entity = await _nutritionService.AddNutritionTarget(userId, nutriotionTarget);

            if (entity == null)
            {
                return NotFound($"NutritionTarget with user Id = {userId} was not added");
            }

            return CreatedAtAction("GetNutritionTargets", new { id = entity.Id }, nutriotionTarget);
        }

        // PUT: api/NutritionTarget/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNutritionTargets(int id, NutritionTargetDTO nutriotionTarget)
        {
            var userId = GetCurrentUserId();
            var target = await _nutritionService.UpdateNutritionTarget(id, userId, nutriotionTarget);

            if (target == null)
            {
                return NotFound($"NutritionTarget with user Id = {userId} was not added.");
            }

            return NoContent();
        }

        // DELETE: api/NutritionTarget/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNutritionTargets(int id)
        {
            var userId = GetCurrentUserId();
            var isDeleted = await _nutritionService.DeleteNutritionTarget(id, userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }


        private int GetCurrentUserId()
        {
            return 1; // temporary user Id
        }
    }
}
