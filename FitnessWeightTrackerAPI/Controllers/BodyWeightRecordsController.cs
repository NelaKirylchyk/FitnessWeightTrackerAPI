using Microsoft.AspNetCore.Mvc;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
 //   [Authorize]
    public class BodyWeightRecordsController : ControllerBase
    {
        private IBodyWeightService _bodyWeightService;
        private IUserService _userService;

        public BodyWeightRecordsController(IBodyWeightService bodyWeightService, IUserService userService)
        {
            _bodyWeightService = bodyWeightService;
            _userService = userService;
        }

        // GET: api/BodyWeightRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BodyWeightRecord>>> GetBodyWeightRecords()
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }

            return await _bodyWeightService.GetAllUserBodyweightRecords((int)userId);
        }

        // GET: api/BodyWeightRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BodyWeightRecord>> GetBodyWeightRecords(int id)
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var bodyWeightRecord = await _bodyWeightService.GetBodyweightRecord(id, (int)userId);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }

            var created = await _bodyWeightService.AddBodyweightRecord((int)userId, bodyWeightRecord);

            if (created == null)
            {
                return NotFound("BodyWeghtRecord was not added.");
            }

            return CreatedAtAction("GetBodyWeightRecords", new
            {
                id = created.Id,
                
            }, bodyWeightRecord);
        }

        // PUT: api/BodyWeightRecord/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBodyWeightRecords(int id, BodyWeightRecordDTO bodyWeightRecord)
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }

            var record = await _bodyWeightService.UpdateBodyweightRecord(id, (int)userId, bodyWeightRecord);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId  == null)
            {
                return Unauthorized();
            }

            var isDeleted = await _bodyWeightService.DeleteBodyweightRecord(id, (int)userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
