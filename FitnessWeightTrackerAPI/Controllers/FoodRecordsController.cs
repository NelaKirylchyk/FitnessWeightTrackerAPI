using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Models.FoodRecords.Commands.CreateFoodRecordCommand;
using FitnessWeightTrackerAPI.Models.FoodRecords.Commands.DeleteFoodRecordCommand;
using FitnessWeightTrackerAPI.Models.FoodRecords.Commands.UpdateFoodRecordCommand;
using FitnessWeightTrackerAPI.Models.FoodRecords.Queries.GetAllFoodRecordsQuery;
using FitnessWeightTrackerAPI.Models.FoodRecords.Queries.GetByIdFoodRecordQuery;
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
    public class FoodRecordsController : BaseController
    {
        private readonly IMediator _mediator;

        public FoodRecordsController(
            IMediator mediator,
            UserManager<FitnessUser> userManager)
            : base(userManager)
        {
            _mediator = mediator;
        }

        // GET: api/FoodRecords
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodRecord>>> GetFoodRecords(bool ascendingOrder = false)
        {
            var userId = GetUserIdAsync();
            var query = new GetAllFoodRecordsQuery
            {
                UserId = userId,
                AscendingOrder = ascendingOrder
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET: api/FoodRecords/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodRecord>> GetFoodRecords(int id)
        {
            var userId = GetUserIdAsync();
            var query = new GetFoodRecordByIdQuery
            {
                Id = id,
                UserId = userId
            };
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/FoodRecords/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFoodRecords(int id, FoodRecordDTO foodRecord)
        {
            var userId = GetUserIdAsync();
            var command = new UpdateFoodRecordCommand
            {
                Id = id,
                UserId = userId,
                Record = foodRecord
            };
            await _mediator.Send(command);
            return NoContent();
        }

        // POST: api/FoodRecords
        [HttpPost]
        public async Task<ActionResult<FoodRecord>> PostFoodRecords(FoodRecordDTO foodRecord)
        {
            var userId = GetUserIdAsync();

            var command = new AddFoodRecordCommand
            {
                UserId = userId,
                Record = foodRecord
            };
            var result = await _mediator.Send(command);

            if (result == null)
            {
                return NotFound($"FoodRecord was not added.");
            }

            return CreatedAtAction(nameof(GetFoodRecords), new { id = result.Id }, result);
        }

        // DELETE: api/FoodRecords/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodRecords(int id)
        {
            var userId = GetUserIdAsync();
            var command = new DeleteFoodRecordCommand
            {
                Id = id,
                UserId = userId
            };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
