using FitnessWeightTrackerAPI.Data.DTO;
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
    public class BodyWeightTargetsController : ControllerBase
    {
        private IBodyWeightTargetService _bodyWeightTargetService;
        private UserManager<FitnessUser> _userManager;

        public BodyWeightTargetsController(
            IBodyWeightTargetService bodyWeightService,
            UserManager<FitnessUser> userManager)
        {
            _bodyWeightTargetService = bodyWeightService;
            _userManager = userManager;
        }

        // GET: api/BodyWeightTargets/5
        [HttpGet]
        public async Task<ActionResult<BodyWeightTarget>> GetBodyWeightTargets()
        {
            var userId = await GetUserIdAsync();

            return await _bodyWeightTargetService.GetUserBodyweightTarget(userId);
        }

        // POST: api/BodyWeightTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BodyWeightTarget>> PostBodyWeightTargets(BodyWeightTargetDTO bodyWeightTarget)
        {
            var userId = await GetUserIdAsync();
            var entity = await _bodyWeightTargetService.AddBodyweightTarget(userId, bodyWeightTarget);

            if (entity == null)
            {
                return NotFound($"BodyWeightTarget with user Id = {userId} was not added");
            }

            return CreatedAtAction("GetBodyWeightTargets", new { id = entity.Id }, bodyWeightTarget);
        }

        // PUT: api/BodyWeightTargets/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBodyWeightTargets(int id, BodyWeightTargetDTO bodyWeightTarget)
        {
            var userId = await GetUserIdAsync();
            await _bodyWeightTargetService.UpdateBodyweightTarget(id, userId, bodyWeightTarget);

            return NoContent();
        }

        // DELETE: api/BodyWeightTargets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightTargets(int id)
        {
            var userId = await GetUserIdAsync();
            await _bodyWeightTargetService.DeleteBodyweightTarget(id, userId);

            return NoContent();
        }

        private async Task<int> GetUserIdAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return user.Id;
        }
    }
}
