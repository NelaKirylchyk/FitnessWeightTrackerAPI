using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using FitnessWeightTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Services
{
    public class NutritionTargetService : INutritionTargetService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<NutritionTargetService> _logger;

        public NutritionTargetService(FitnessWeightTrackerDbContext context, ILogger<NutritionTargetService> logger)
        {
            _context = context;
            _logger = logger;
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
            }

            return entity;
        }

        public async Task DeleteNutritionTarget(int id, int userId)
        {
            await _context.NutritionTargets.Where(t => t.UserId == userId && t.Id == id).ExecuteDeleteAsync();
        }

        public async Task<NutritionTarget> GetNutritionTarget(int userId)
        {
            var record = await _context.NutritionTargets.AsNoTracking().FirstOrDefaultAsync(r => r.UserId == userId);
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
        }

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
