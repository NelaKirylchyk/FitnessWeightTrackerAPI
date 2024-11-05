using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Services.Interfaces;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    public class FoodRecordsController : ControllerBase
    {
        private INutritionService _nutritionService;
        private IUserService _userService;

        public FoodRecordsController(INutritionService nutritionService, IUserService userService)
        {
            _nutritionService = nutritionService;
            _userService = userService;
        }

        // GET: api/FoodRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodRecord>>> GetFoodRecords()
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            return await _nutritionService.GetAllFoodRecords((int)userId);
        }

        // GET: api/FoodRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodRecord>> GetFoodRecords(int id)
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var foodRecord = await _nutritionService.GetFoodRecord(id, (int)userId);

            if (foodRecord == null)
            {
                return NotFound();
            }

            return foodRecord;
        }

        // PUT: api/FoodRecords/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodRecords(int id, FoodRecordDTO foodRecord)
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var record = await _nutritionService.UpdateFoodRecord(id, (int)userId, foodRecord);

            if (record == null)
            {
                return NotFound($"FoodRecord with Id = {id} was not updated.");
            }

            return NoContent();
        }

        // POST: api/FoodRecords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FoodRecord>> PostFoodRecords(FoodRecordDTO foodRecord)
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var created = await _nutritionService.AddFoodRecord(foodRecord, (int)userId);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var isDeleted = await _nutritionService.DeleteFoodRecord(id, (int)userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
