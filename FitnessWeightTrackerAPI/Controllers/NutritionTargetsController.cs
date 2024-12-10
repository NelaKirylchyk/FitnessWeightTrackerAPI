using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.CreateNutritionTargetCommand;
using FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.DeleteNutritionTargetCommand;
using FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.UpdateNutritionTargetCommand;
using FitnessWeightTrackerAPI.Models.NutritionTargets.Queries.GetNutritionTargetByIdQuery;
using FitnessWeightTrackerAPI.Models.NutritionTargets.Queries.GetNutritionTargetQuery;
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
    public class NutritionTargetsController : BaseController
    {
        private readonly IMediator _mediator;

        public NutritionTargetsController(
            IMediator mediator,
            UserManager<FitnessUser> userManager)
            : base(userManager)
        {
            _mediator = mediator;
        }

        // GET: api/NutritionTargets/5
        [HttpGet]
        public async Task<ActionResult<NutritionTarget>> GetNutritionTargets()
        {
            var userId = await GetUserIdAsync();
            var query = new GetNutritionTargetQuery
            {
                UserId = userId
            };
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // GET: api/NutritionTargets/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<NutritionTarget>> GetNutritionTargetById(int id)
        {
            var userId = await GetUserIdAsync();
            var query = new GetNutritionTargetByIdQuery
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

        // POST: api/NutritionTargets
        [HttpPost]
        public async Task<ActionResult<NutritionTarget>> PostNutritionTargets(NutritionTargetDTO nutriotionTarget)
        {
            var userId = await GetUserIdAsync();

            try
            {
                var command = new AddNutritionTargetCommand
                {
                    UserId = userId,
                    Target = nutriotionTarget
                };
                var result = await _mediator.Send(command);
                if (result == null)
                {
                    return NotFound($"NutritionTarget with user Id = {userId} was not added");
                }

                return CreatedAtAction(nameof(GetNutritionTargets), new { id = result.Id }, result);
            }
            catch (NutritionTargetAlreadyExistsException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // PUT: api/NutritionTarget/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNutritionTargets(int id, NutritionTargetDTO nutriotionTarget)
        {
            var userId = await GetUserIdAsync();
            var command = new UpdateNutritionTargetCommand
            {
                Id = id,
                UserId = userId,
                Target = nutriotionTarget
            };
            await _mediator.Send(command);
            return NoContent();
        }

        // DELETE: api/NutritionTarget/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNutritionTargets(int id)
        {
            var userId = await GetUserIdAsync();
            var command = new DeleteNutritionTargetCommand
            {
                Id = id,
                UserId = userId
            };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
