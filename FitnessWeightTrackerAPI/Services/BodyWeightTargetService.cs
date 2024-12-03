using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Services
{
    public class BodyWeightTargetService : IBodyWeightTargetService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<BodyWeightTargetService> _logger;
        private readonly IDistributedCache _cache;

        public BodyWeightTargetService(FitnessWeightTrackerDbContext context, ILogger<BodyWeightTargetService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<BodyWeightTarget> GetUserBodyweightTarget(int userId)
        {
            var cacheKey = $"BodyWeightTarget_{userId}";
            var cachedTarget = await _cache.GetStringAsync(cacheKey);
            if (cachedTarget != null)
            {
                return JsonSerializer.Deserialize<BodyWeightTarget>(cachedTarget);
            }

            var target = await _context.BodyWeightTargets.AsNoTracking().FirstOrDefaultAsync(target => target.UserId == userId);

            if (target != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(target));
            }

            return target;
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

                await _cache.RemoveAsync($"BodyWeightTarget_{userId}"); // Invalidate cache
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

            await _cache.RemoveAsync($"BodyWeightTarget_{userId}"); // Invalidate cache
        }

        public async Task DeleteBodyweightTarget(int id, int userId)
        {
            await _context.BodyWeightTargets.Where(t => t.UserId == userId && t.Id == id).ExecuteDeleteAsync();
            _logger.LogInformation("BodyWeightTarget was deleted.");

            await _cache.RemoveAsync($"BodyWeightTarget_{userId}"); // Invalidate cache
        }

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
