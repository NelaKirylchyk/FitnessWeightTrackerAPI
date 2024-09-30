using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services;
using FitnessWeightTrackerAPI.Data.DTO;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FitnessWeightTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BodyWeightRecordsController : ControllerBase
    {
        private IBodyWeightService _bodyWeightService;

        public BodyWeightRecordsController(IBodyWeightService bodyWeightService)
        {
            _bodyWeightService = bodyWeightService;
        }

        // GET: api/BodyWeightRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BodyWeightRecord>>> GetBodyWeightRecords()
        {
            int userId = GetCurrentUserId();
            return await _bodyWeightService.GetAllUserBodyweightRecords(userId);
        }

        // GET: api/BodyWeightRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BodyWeightRecord>> GetBodyWeightRecord(int id)
        {
            int userId = GetCurrentUserId();
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
        public async Task<ActionResult<BodyWeightRecord>> PostBodyWeightRecord(BodyWeightRecordDTO bodyWeightRecord)
        {
            int userId = GetCurrentUserId();
            var created = await _bodyWeightService.AddBodyweightRecord(userId, bodyWeightRecord);

            if (created == null)
            {
                return NotFound($"BodyWeghtRecord was not added.");
            }

            return CreatedAtAction("GetBodyWeightRecord", new { id = created.Id }, bodyWeightRecord);

        }

        // PUT: api/BodyWeightRecord/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBodyWeightRecord(int id, BodyWeightRecordDTO bodyWeightRecord)
        {
            var userId = GetCurrentUserId();
            var record = await _bodyWeightService.UpdateBodyweightRecord(id, userId, bodyWeightRecord);

            if (record == null)
            {
                return NotFound($"BodyWeghtRecord with Id = {id} was not updated.");
            }

            return NoContent();
        }


        // DELETE: api/BodyWeightRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightRecord(int id)
        {
            int userId = GetCurrentUserId();
            var isDeleted = await _bodyWeightService.DeleteBodyweightRecord(id, userId);
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
