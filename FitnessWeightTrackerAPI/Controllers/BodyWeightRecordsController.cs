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
            // Retrieve the user ID from the claims
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            return await _bodyWeightService.GetAllUserBodyweightRecords(userId);
        }

        // GET: api/BodyWeightRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BodyWeightRecord>> GetBodyWeightRecords(int id)
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var record = await _bodyWeightService.UpdateBodyweightRecord(id, userId, bodyWeightRecord);

            if (record == null)
            {
                return NotFound($"BodyWeghtRecord with Id = {id} was not updated.");
            }

            return NoContent();
        }

        // DELETE: api/BodyWeightRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightRecords(int id)
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            var isDeleted = await _bodyWeightService.DeleteBodyweightRecord(id, userId);
            if (!isDeleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
