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
    public class BodyWeightTargetsController : ControllerBase
    {
        private IBodyWeightTargetService _bodyWeightTargetService;

        public BodyWeightTargetsController(IBodyWeightTargetService bodyWeightService)
        {
            _bodyWeightTargetService = bodyWeightService;
        }

        // GET: api/BodyWeightTargets/5
        [HttpGet]
        public async Task<ActionResult<BodyWeightTarget>> GetBodyWeightTargets()
        {
            var userId = GetUserIdFromClaim();

            return await _bodyWeightTargetService.GetUserBodyweightTarget(userId);
        }

        // POST: api/BodyWeightTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BodyWeightTarget>> PostBodyWeightTargets(BodyWeightTargetDTO bodyWeightTarget)
        {
            var userId = GetUserIdFromClaim();
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
            var userId = GetUserIdFromClaim();
            await _bodyWeightTargetService.UpdateBodyweightTarget(id, userId, bodyWeightTarget);

            return NoContent();
        }

        // DELETE: api/BodyWeightTargets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightTargets(int id)
        {
            var userId = GetUserIdFromClaim();
            await _bodyWeightTargetService.DeleteBodyweightTarget(id, userId);

            return NoContent();
        }

        private int GetUserIdFromClaim()
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }
    }
}
