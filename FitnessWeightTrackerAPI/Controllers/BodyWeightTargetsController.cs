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
using FitnessWeightTrackerAPI.Filters;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    public class BodyWeightTargetsController : ControllerBase
    {
        private IBodyWeightService _bodyWeightService;

        public BodyWeightTargetsController(IBodyWeightService bodyWeightService)
        {
            _bodyWeightService = bodyWeightService;
        }

        // GET: api/BodyWeightTargets/5
        [HttpGet]
        public async Task<ActionResult<BodyWeightTarget>> GetBodyWeightTargets()
        {
            int userId = GetCurrentUserId();
            var bodyWeightTarget = await _bodyWeightService.GetUserBodyweightTarget(userId);

            if (bodyWeightTarget == null)
            {
                return NotFound();
            }

            return bodyWeightTarget;
        }

        // POST: api/BodyWeightTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BodyWeightTarget>> PostBodyWeightTargets(BodyWeightTargetDTO bodyWeightTarget)
        {
            var userId = GetCurrentUserId();
            var entity = await _bodyWeightService.AddBodyweightTarget(userId, bodyWeightTarget);

            if (entity == null)
            {
                return NotFound($"BodyWeightTarget with user Id = {userId} was not added");
            }

            return CreatedAtAction("GetBodyWeightTargets", new { id = entity.Id }, bodyWeightTarget);
        }

        // PUT: api/BodyWeightTargets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBodyWeightTargets(int id, BodyWeightTargetDTO bodyWeightTarget)
        {
            var userId = GetCurrentUserId();
            var record = await _bodyWeightService.UpdateBodyweightTarget(id, userId, bodyWeightTarget);

            if (record == null)
            {
                return NotFound($"BodyWeghtTarget with user Id = {userId} was not added.");
            }

            return NoContent();
        }

        // DELETE: api/BodyWeightTargets/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBodyWeightTargets(int id)
        {
            var userId = GetCurrentUserId();
            var isDeleted = await _bodyWeightService.DeleteBodyweightTarget(id, userId);
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
