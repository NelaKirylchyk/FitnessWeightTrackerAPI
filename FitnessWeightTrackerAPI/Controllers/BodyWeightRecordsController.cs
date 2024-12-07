﻿using FitnessWeightTrackerAPI.Data.DTO;
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
    public class BodyWeightRecordsController : BaseController
    {
        private IBodyWeightService _bodyWeightService;

        public BodyWeightRecordsController(
            IBodyWeightService bodyWeightService,
            UserManager<FitnessUser> userManager)
            : base(userManager)
        {
            _bodyWeightService = bodyWeightService;
        }

        // GET: api/BodyWeightRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BodyWeightRecord>>> GetBodyWeightRecords()
        {
            var userId = await GetUserIdAsync();

            return await _bodyWeightService.GetAllUserBodyweightRecords(userId);
        }

        // GET: api/BodyWeightRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BodyWeightRecord>> GetBodyWeightRecords(int id)
        {
            var userId = await GetUserIdAsync();

            var bodyWeightRecord = await _bodyWeightService.GetBodyweightRecord(id, userId);

            if (bodyWeightRecord == null)
            {
                return NotFound();
            }

            return bodyWeightRecord;
        }

        // POST: api/BodyWeightRecords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BodyWeightRecord>> PostBodyWeightRecords(BodyWeightRecordDTO bodyWeightRecord)
        {
            var userId = await GetUserIdAsync();

            var created = await _bodyWeightService.AddBodyweightRecord(userId, bodyWeightRecord);

            if (created == null)
            {
                return NotFound("BodyWeghtRecord was not added.");
            }

            return CreatedAtAction("GetBodyWeightRecords", new
            {
                id = created.Id,
            },
            bodyWeightRecord);
        }

        // PUT: api/BodyWeightRecord/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBodyWeightRecords(int id, BodyWeightRecordDTO bodyWeightRecord)
        {
            var userId = await GetUserIdAsync();

            await _bodyWeightService.UpdateBodyweightRecord(id, userId, bodyWeightRecord);

            return NoContent();
        }

        // DELETE: api/BodyWeightRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightRecords(int id)
        {
            var userId = await GetUserIdAsync();

            await _bodyWeightService.DeleteBodyweightRecord(id, userId);

            return NoContent();
        }
    }
}
