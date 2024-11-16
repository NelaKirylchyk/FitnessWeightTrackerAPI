using System.Security.Claims;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FoodRecordsController : ControllerBase
    {
        private IFoodRecordService _nutritionService;

        public FoodRecordsController(IFoodRecordService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        // GET: api/FoodRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodRecord>>> GetFoodRecords()
        {
            var userId = GetUserIdFromClaim();
            return await _nutritionService.GetAllFoodRecords(userId);
        }

        // GET: api/FoodRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodRecord>> GetFoodRecords(int id)
        {
            var userId = GetUserIdFromClaim();
            var foodRecord = await _nutritionService.GetFoodRecord(id, userId);

            if (foodRecord == null)
            {
                return NotFound();
            }

            return foodRecord;
        }

        // PUT: api/FoodRecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodRecords(int id, FoodRecordDTO foodRecord)
        {
            var userId = GetUserIdFromClaim();
            await _nutritionService.UpdateFoodRecord(id, userId, foodRecord);

            return NoContent();
        }

        // POST: api/FoodRecords
        [HttpPost]
        public async Task<ActionResult<FoodRecord>> PostFoodRecords(FoodRecordDTO foodRecord)
        {
            var userId = GetUserIdFromClaim();
            var created = await _nutritionService.AddFoodRecord(foodRecord, userId);

            if (created == null)
            {
                return NotFound($"FoodRecord was not added.");
            }

            return CreatedAtAction("GetFoodRecords", new { id = created.Id }, foodRecord);
        }

        // DELETE: api/FoodRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodRecords(int id)
        {
            var userId = GetUserIdFromClaim();
            await _nutritionService.DeleteFoodRecord(id, userId);

            return NoContent();
        }

        private int GetUserIdFromClaim()
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }
    }
}
