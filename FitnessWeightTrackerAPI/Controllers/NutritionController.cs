using Microsoft.AspNetCore.Mvc;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services;
using FitnessWeightTrackerAPI.Data.DTO;

namespace FitnessWeightTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NutritionController : ControllerBase
    {
        private INutritionService _nutritionService;

        public NutritionController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        // GET: api/NutritionFoodRecords
        [HttpGet("GetFoodRecords")]
        public async Task<ActionResult<IEnumerable<FoodRecord>>> GetFoodRecords()
        {
            int userId = GetCurrentUserId();
            return await _nutritionService.GetAllFoodRecords(userId);
        }

        // GET: api/NutritionFoodRecord/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodRecord>> GetFoodRecord(int id)
        {
            int userId = GetCurrentUserId();
            var foodRecord = await _nutritionService.GetFoodRecord(id, userId);

            if (foodRecord == null)
            {
                return NotFound();
            }

            return foodRecord;
        }

        // PUT: api/NutritionFoodRecord/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutFoodRecord/{id}")]
        public async Task<IActionResult> PutFoodRecord(int id, FoodRecordDTO foodRecord)
        {
            var userId = GetCurrentUserId();
            var record = await _nutritionService.UpdateFoodRecord(id, userId, foodRecord);

            if (record == null)
            {
                return NotFound($"FoodRecord with Id = {id} was not updated.");
            }

            return NoContent();
        }

        // POST: api/NutritionFoodRecord
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostFoodRecord")]
        public async Task<ActionResult<FoodRecord>> PostFoodRecord(FoodRecordDTO foodRecord)
        {
            int userId = GetCurrentUserId();
            var created = await _nutritionService.AddFoodRecord(foodRecord, userId);

            if (created == null)
            {
                return NotFound($"FoodRecord was not added.");
            }

            return CreatedAtAction("GetFoodRecord", new { id = created.Id }, foodRecord);
        }

        // DELETE: api/NutritionFoodRecord/5
        [HttpDelete("DeleteFoodRecord/{id}")]
        public async Task<IActionResult> DeleteFoodRecord(int id)
        {
            int userId = GetCurrentUserId();
            var isDeleted = await _nutritionService.DeleteFoodRecord(id, userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        // GET: api/NutritionTarget/5
        [HttpGet("GetNutritionTarget")]
        public async Task<ActionResult<NutritionTarget>> GetNutritionTarget()
        {
            int userId = GetCurrentUserId();
            var nutritionTarget = await _nutritionService.GetNutritionTarget(userId);

            if (nutritionTarget == null)
            {
                return NotFound();
            }

            return nutritionTarget;
        }

        // POST: api/NutritionTarget
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PostNutritionTarget")]
        public async Task<ActionResult<NutritionTarget>> PostNutritionTarget(NutritionTargetDTO nutriotionTarget)
        {
            var userId = GetCurrentUserId();
            var entity = await _nutritionService.AddNutritionTarget(userId, nutriotionTarget);

            if (entity == null)
            {
                return NotFound($"NutritionTarget with user Id = {userId} was not added");
            }

            return CreatedAtAction("GetNutritionTarget", new { id = entity.Id }, nutriotionTarget);
        }

        // PUT: api/NutritionTarget/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("PutNutritionTarget/{id}")]
        public async Task<IActionResult> PutNutritionTarget(int id, NutritionTargetDTO nutriotionTarget)
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
        [HttpDelete("DeleteNutritionTarget/{id}")]
        public async Task<IActionResult> DeleteNutritionTarget(int id)
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
