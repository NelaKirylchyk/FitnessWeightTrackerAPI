﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    public class FoodRecordsController : ControllerBase
    {
        private INutritionService _nutritionService;

        public FoodRecordsController(INutritionService nutritionService)
        {
            _nutritionService = nutritionService;
        }

        // GET: api/FoodRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodRecord>>> GetFoodRecords()
        {
            int userId = GetCurrentUserId();
            return await _nutritionService.GetAllFoodRecords(userId);
        }

        // GET: api/FoodRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodRecord>> GetFoodRecords(int id)
        {
            int userId = GetCurrentUserId();
            var foodRecord = await _nutritionService.GetFoodRecord(id, userId);

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
            var userId = GetCurrentUserId();
            var record = await _nutritionService.UpdateFoodRecord(id, userId, foodRecord);

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
            int userId = GetCurrentUserId();
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
            int userId = GetCurrentUserId();
            var isDeleted = await _nutritionService.DeleteFoodRecord(id, userId);
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
