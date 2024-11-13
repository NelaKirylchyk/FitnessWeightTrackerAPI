﻿using System.Security.Claims;
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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            return await _nutritionService.GetAllFoodRecords(userId);
        }

        // GET: api/FoodRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodRecord>> GetFoodRecords(int id)
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var record = await _nutritionService.UpdateFoodRecord(id, userId, foodRecord);

            if (record == null)
            {
                return NotFound($"FoodRecord with Id = {id} was not updated.");
            }

            return NoContent();
        }

        // POST: api/FoodRecords
        [HttpPost]
        public async Task<ActionResult<FoodRecord>> PostFoodRecords(FoodRecordDTO foodRecord)
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var isDeleted = await _nutritionService.DeleteFoodRecord(id, userId);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
