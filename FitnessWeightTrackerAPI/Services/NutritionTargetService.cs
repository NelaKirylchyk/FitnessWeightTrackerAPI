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

        public NutritionTargetService(FitnessWeightTrackerDbContext context)
        {
            _context = context;
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

        public async Task<bool> DeleteNutritionTarget(int id, int userId)
        {
            var target = await _context.NutritionTargets.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);
            if (target != null)
            {
                _context.NutritionTargets.Remove(target);
                await _context.SaveChangesAsync();
            }

            return target != null;
        }

        public async Task<NutritionTarget> GetNutritionTarget(int userId)
        {
            var record = await _context.NutritionTargets.AsNoTracking().FirstOrDefaultAsync(r => r.UserId == userId);
            return record;
        }

        public async Task<NutritionTarget> UpdateNutritionTarget(int id, int userId, NutritionTargetDTO target)
        {
            var userExists = await UserExists(userId);
            var nutritionTarget = await _context.NutritionTargets.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);

            if (userExists && nutritionTarget != null)
            {
                nutritionTarget.DailyProtein = target.DailyProtein;
                nutritionTarget.DailyFat = target.DailyFat;
                nutritionTarget.DailyCarbonohydrates = target.DailyCarbonohydrates;
                nutritionTarget.DailyCalories = target.DailyCalories;

                // Validate updated entity
                if (!ValidationHelper.TryValidateObject(nutritionTarget, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.NutritionTargets.Update(nutritionTarget);
                await _context.SaveChangesAsync();
            }

            return nutritionTarget;
        }

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
