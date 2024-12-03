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
    public class NutritionTargetService : INutritionTargetService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<NutritionTargetService> _logger;
        private readonly IDistributedCache _cache;

        public NutritionTargetService(FitnessWeightTrackerDbContext context, ILogger<NutritionTargetService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<NutritionTarget> AddNutritionTarget(int userId, NutritionTargetDTO target)
        {
            var userExists = await UserExists(userId);
            var anyUserTargetExists = await _context.NutritionTargets.AnyAsync(t => t.UserId == userId);

            NutritionTarget? entity = null;

            if (userExists && !anyUserTargetExists)
            {
                entity = new NutritionTarget()
                {
                    UserId = userId,
                    DailyCalories = target.DailyCalories,
                    DailyCarbonohydrates = target.DailyCarbonohydrates,
                    DailyFat = target.DailyFat,
                    DailyProtein = target.DailyProtein
                };

                // Validate entity
                if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.NutritionTargets.Add(entity);
                await _context.SaveChangesAsync();

                await _cache.RemoveAsync($"NutritionTarget_{userId}"); // Invalidate cache
            }

            return entity;
        }

        public async Task DeleteNutritionTarget(int id, int userId)
        {
            await _context.NutritionTargets.Where(t => t.UserId == userId && t.Id == id).ExecuteDeleteAsync();

            await _cache.RemoveAsync($"NutritionTarget_{userId}"); // Invalidate cache
        }

        public async Task<NutritionTarget> GetNutritionTarget(int userId)
        {
            var cacheKey = $"NutritionTarget_{userId}";
            var cachedRecord = await _cache.GetStringAsync(cacheKey);
            if (cachedRecord != null)
            {
                return JsonSerializer.Deserialize<NutritionTarget>(cachedRecord);
            }

            var record = await _context.NutritionTargets.AsNoTracking().FirstOrDefaultAsync(r => r.UserId == userId);

            if (record != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(record));
            }

            return record;
        }

        public async Task UpdateNutritionTarget(int id, int userId, NutritionTargetDTO target)
        {
            if (!ValidationHelper.TryValidateObject(target, out var validationResults))
            {
                throw new CustomValidationException(validationResults);
            }

            var userExists = await UserExists(userId);
            var foodRecord = await _context.NutritionTargets.Where(t => t.UserId == userId && t.Id == id)
                  .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.DailyProtein, target.DailyProtein)
                .SetProperty(b => b.DailyFat, target.DailyFat)
                .SetProperty(b => b.DailyCarbonohydrates, target.DailyCarbonohydrates)
                .SetProperty(b => b.DailyCalories, target.DailyCalories));

            await _cache.RemoveAsync($"NutritionTarget_{userId}"); // Invalidate cache
        }

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
