using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Services
{
    public class NutritionService : INutritionService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        public NutritionService(FitnessWeightTrackerDbContext context) 
        {
            _context = context;
        }
        public async Task<FoodRecord> AddFoodRecord(FoodRecordDTO foodRecord, int userId)
        {
            var userExists = await UserExists(userId);
            var foodItemExists = _context.FoodItems.Any(x => x.Id == foodRecord.FoodItemId);
            FoodRecord? entity = null;

            if (userExists && foodItemExists)
            {
                entity = new FoodRecord()
                {
                    UserId = userId,
                    ConsumptionDate = foodRecord.ConsumptionDate,
                    FoodItemId = foodRecord.FoodItemId,
                    Quantity = foodRecord.Quantity
                };

                // Validate  entity
                if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.FoodRecords.Add(entity);
                await _context.SaveChangesAsync();
            }

            return entity;
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

        public async Task DeleteAllFoodRecords(int userId)
        {

            FoodRecord[] existingRecords = await _context.FoodRecords.Where(x => x.UserId == userId).ToArrayAsync();
            _context.FoodRecords.RemoveRange(existingRecords);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteFoodRecord(int id, int userId)
        {
            var existingRecord = await _context.FoodRecords.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (existingRecord != null)
            {
                _context.FoodRecords.Remove(existingRecord);
                await _context.SaveChangesAsync();
            }
            return existingRecord != null;
        }

        public async Task<FoodRecord[]> GetAllFoodRecords(int userId, bool AscendingOrder = false)
        {
            FoodRecord[] records = null;

            if (AscendingOrder == false)
            {
                records = await _context.FoodRecords
                    .Where(record => record.UserId == userId)
                    .OrderByDescending(record => record.ConsumptionDate)
                    .ToArrayAsync();
            }
            else
            {
                records = await _context.FoodRecords
                     .Where(record => record.UserId == userId)
                    .OrderBy(record => record.ConsumptionDate)
                    .ToArrayAsync();
            }

            return records;
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


        public async Task<FoodRecord> GetFoodRecord(int id, int userId)
        {
            var record = await _context.FoodRecords.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            return record;
        }

        public async Task<NutritionTarget> GetNutritionTarget(int userId)
        {
            var record = await _context.NutritionTargets.FirstOrDefaultAsync(r => r.UserId == userId);
            return record;
        }

        public async Task<FoodRecord> UpdateFoodRecord(int id, int userId, FoodRecordDTO record)
        {
            var userExists = await UserExists(userId);
            var foodRecord = await _context.FoodRecords.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);


            if (userExists && foodRecord != null)
            {
                foodRecord.ConsumptionDate = record.ConsumptionDate;
                foodRecord.Quantity = record.Quantity;
                foodRecord.FoodItemId = record.FoodItemId;

                // Validate updated entity
                if (!ValidationHelper.TryValidateObject(foodRecord, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.FoodRecords.Update(foodRecord);
                await _context.SaveChangesAsync();
            }
            return foodRecord;
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
            var user = await _context.Users.FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
