using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Services.Interfaces;
using FitnessWeightTrackerAPI.Services;

namespace FitnessWeightTrackerAPI.Controllers
{
    [ValidateModel]
    [Route("api/[controller]")]
    [ApiController]
    public class BodyWeightTargetsController : ControllerBase
    {
        private IBodyWeightService _bodyWeightService;
        private IUserService _userService;

        public BodyWeightTargetsController(IBodyWeightService bodyWeightService, IUserService userService)
        {
            _bodyWeightService = bodyWeightService;
            _userService = userService;
        }

        // GET: api/BodyWeightTargets/5
        [HttpGet]
        public async Task<ActionResult<BodyWeightTarget>> GetBodyWeightTargets()
        {
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }

            var bodyWeightTarget = await _bodyWeightService.GetUserBodyweightTarget((int)userId);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var entity = await _bodyWeightService.AddBodyweightTarget((int)userId, bodyWeightTarget);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var record = await _bodyWeightService.UpdateBodyweightTarget(id, (int)userId, bodyWeightTarget);

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
            var userId = _userService.GetUserIdFromAuth(this);

            if (userId == null)
            {
                return Unauthorized();
            }
            var isDeleted = await _bodyWeightService.DeleteBodyweightTarget(id, (int)userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
