using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Services
{
    public class BodyWeightTargetService : IBodyWeightTargetService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<BodyWeightTargetService> _logger;

        public BodyWeightTargetService(FitnessWeightTrackerDbContext context, ILogger<BodyWeightTargetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BodyWeightTarget> GetUserBodyweightTarget(int userId)
        {
            var result = await _context.BodyWeightTargets.AsNoTracking().FirstOrDefaultAsync(target => target.UserId == userId);
            return result;
        }

        public async Task<BodyWeightTarget> AddBodyweightTarget(int userId, BodyWeightTargetDTO targetWeight)
        {
            var userExists = await UserExists(userId);

            BodyWeightTarget? entity = null;

            if (userExists)
            {
                entity = new BodyWeightTarget()
                {
                    UserId = userId,
                    TargetDate = targetWeight.TargetDate,
                    TargetWeight = targetWeight.TargetWeight
                };

                // Validate updated entity
                if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.BodyWeightTargets.Add(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("BodyWeightTarget was added.");
            }

            return entity;
        }

        public async Task UpdateBodyweightTarget(int id, int userId, BodyWeightTargetDTO targetRecord)
        {
            if (!ValidationHelper.TryValidateObject(targetRecord, out var validationResults))
            {
                _logger.LogError("Validation Error while updating BodyWeightTarget.");
                throw new CustomValidationException(validationResults);
            }

            await _context.BodyWeightTargets.Where(t => t.UserId == userId && t.Id == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.TargetWeight, targetRecord.TargetWeight)
                .SetProperty(b => b.TargetDate, targetRecord.TargetDate));
            _logger.LogInformation("BodyWeightTarget was updated.");
        }

        public async Task DeleteBodyweightTarget(int id, int userId)
        {
            await _context.BodyWeightTargets.Where(t => t.UserId == userId && t.Id == id).ExecuteDeleteAsync();
            _logger.LogInformation("BodyWeightTarget was deleted.");
        }

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
