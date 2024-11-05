using Microsoft.AspNetCore.Mvc;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Services.Interfaces;
using FitnessWeightTrackerAPI.Services;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionTargetsController : ControllerBase
    {
        private INutritionService _nutritionService;
        private IUserService _userService;

        public NutritionTargetsController(INutritionService nutritionService, IUserService userService)
        {
            _nutritionService = nutritionService;
            _userService = userService;
        }

        // GET: api/NutritionTargets/5
        [HttpGet]
        public async Task<ActionResult<NutritionTarget>> GetNutritionTargets()
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            return await _nutritionService.GetNutritionTarget((int)userId);
        }

        // POST: api/NutritionTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NutritionTarget>> PostNutritionTargets(NutritionTargetDTO nutriotionTarget)
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var entity = await _nutritionService.AddNutritionTarget((int)userId, nutriotionTarget);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var target = await _nutritionService.UpdateNutritionTarget(id, (int)userId, nutriotionTarget);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var isDeleted = await _nutritionService.DeleteNutritionTarget(id, (int)userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
