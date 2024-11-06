﻿using Microsoft.AspNetCore.Mvc;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Filters;
using FitnessWeightTrackerAPI.Services.Interfaces;
using System.Security.Claims;

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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);

            return await _bodyWeightService.GetUserBodyweightTarget(userId);
        }

        // POST: api/BodyWeightTargets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BodyWeightTarget>> PostBodyWeightTargets(BodyWeightTargetDTO bodyWeightTarget)
        {
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
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
            var userIdClaim = this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = int.Parse(userIdClaim.Value);
            var isDeleted = await _bodyWeightService.DeleteBodyweightTarget(id, userId);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
