using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.CreateBodyWeightRecordCommand;
using FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.DeleteBodyWeightRecordCommand;
using FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.UpdateBodyWeightRecordCommand;
using FitnessWeightTrackerAPI.Models.BodyWeightRecords.Queries.GetAllBodyWeightRecordsQuery;
using FitnessWeightTrackerAPI.Models.BodyWeightRecords.Queries.GetByIdBodyWeightRecordsQuery;
using MediatR;
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
        private readonly IMediator _mediator;

        public BodyWeightRecordsController(
            IMediator mediator,
            UserManager<FitnessUser> userManager)
            : base(userManager)
        {
            _mediator = mediator;
        }

        // GET: api/BodyWeightRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BodyWeightRecord>>> GetBodyWeightRecords([FromQuery] bool ascendingOrder = false)
        {
            var userId = await GetUserIdAsync();

            var query = new GetBodyWeightRecordsQuery { UserId = userId, AscendingOrder = ascendingOrder };
            var result = await _mediator.Send(query);

            return Ok(result);
        }

        // GET: api/BodyWeightRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BodyWeightRecord>> GetBodyWeightRecords(int id)
        {
            var userId = await GetUserIdAsync();

            var query = new GetBodyWeightRecordByIdQuery { Id = id, UserId = userId };
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/BodyWeightRecords
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BodyWeightRecord>> PostBodyWeightRecords(BodyWeightRecordDTO bodyWeightRecord)
        {
            var userId = await GetUserIdAsync();

            var command = new AddBodyWeightRecordCommand { UserId = userId, Record = bodyWeightRecord };

            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound("BodyWeghtRecord was not added.");
            }

            return CreatedAtAction(nameof(GetBodyWeightRecords), new { id = result.Id }, result);
        }

        // PUT: api/BodyWeightRecord/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBodyWeightRecords(int id, BodyWeightRecordDTO bodyWeightRecord)
        {
            var userId = await GetUserIdAsync();

            var command = new UpdateBodyWeightRecordCommand { Id = id, UserId = userId, Record = bodyWeightRecord };
            var updated = await _mediator.Send(command);

            if (!updated)
                return NotFound();

            return NoContent();
        }

        // DELETE: api/BodyWeightRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightRecords(int id)
        {
            var userId = await GetUserIdAsync();

            var command = new DeleteBodyWeightRecordCommand { Id = id, UserId = userId };
            var deleted = await _mediator.Send(command);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
