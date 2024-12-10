using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.CreateBodyWeightTargetCommand;
using FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.DeleteBodyWeightTargetCommand;
using FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.UpdateBodyWeightTargetCommand;
using FitnessWeightTrackerAPI.Models.BodyWeightTargets.Queries.GetBodyWeightTargetByIdQuery;
using FitnessWeightTrackerAPI.Models.BodyWeightTargets.Queries.GetBodyWeightTargetQuery;
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
    public class BodyWeightTargetsController : BaseController
    {
        private readonly IMediator _mediator;

        public BodyWeightTargetsController(
            IMediator mediator,
            UserManager<FitnessUser> userManager)
            : base(userManager)
        {
            _mediator = mediator;
        }

        // GET: api/BodyWeightTargets/5
        [HttpGet]
        public async Task<ActionResult<BodyWeightTarget>> GetBodyWeightTargets()
        {
            var userId = await GetUserIdAsync();

            var query = new GetBodyWeightTargetQuery { UserId = userId };
            var result = await _mediator.Send(query);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET: api/BodyWeightTargets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<BodyWeightTarget>> GetBodyWeightTargetById(int id)
        {
            var userId = await GetUserIdAsync();
            var query = new GetBodyWeightTargetByIdQuery
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

        // POST: api/BodyWeightTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BodyWeightTarget>> PostBodyWeightTargets(BodyWeightTargetDTO bodyWeightTarget)
        {
            var userId = await GetUserIdAsync();

            try
            {
                var command = new AddBodyWeightTargetCommand
                {
                    UserId = userId,
                    Target = bodyWeightTarget
                };
                var result = await _mediator.Send(command);

                if (result == null)
                {
                    return NotFound($"BodyWeightTarget with user Id = {userId} was not added");
                }

                return CreatedAtAction(nameof(GetBodyWeightTargets), new { id = result.Id }, result);
            }
            catch (TargetAlreadyExistsException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/BodyWeightTargets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBodyWeightTargets(int id, BodyWeightTargetDTO bodyWeightTarget)
        {
            var userId = await GetUserIdAsync();
            var command = new UpdateBodyWeightTargetCommand
            {
                Id = id,
                UserId = userId,
                Target = bodyWeightTarget
            };

            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE: api/BodyWeightTargets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightTargets(int id)
        {
            var userId = await GetUserIdAsync();
            var command = new DeleteBodyWeightTargetCommand
            {
                Id = id,
                UserId = userId
            };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
