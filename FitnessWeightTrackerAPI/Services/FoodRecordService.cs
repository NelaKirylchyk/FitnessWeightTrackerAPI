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
    public class FoodRecordService : IFoodRecordService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<FoodRecordService> _logger;
        private readonly IDistributedCache _cache;

        public FoodRecordService(FitnessWeightTrackerDbContext context, ILogger<FoodRecordService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<FoodRecord> AddFoodRecord(FoodRecordDTO foodRecord, int userId)
        {
            var userExists = await UserExists(userId);
            var foodItemExists = _context.FoodItems.AsNoTracking().Any(x => x.Id == foodRecord.FoodItemId);
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
                    _logger.LogError("Validation error while adding FoodRecord.");
                    throw new CustomValidationException(validationResults);
                }

                _context.FoodRecords.Add(entity);
                await _context.SaveChangesAsync();
                _logger.LogInformation("FoodRecord was added.");

                // Invalidate cache
                await _cache.RemoveAsync($"FoodRecords_{userId}");
            }

            return entity;
        }

        public async Task DeleteAllFoodRecords(int userId)
        {
            await _context.FoodRecords.Where(x => x.UserId == userId).ExecuteDeleteAsync();
            _logger.LogInformation("All FoodRecords were deleted.");

            // Invalidate cache
            await _cache.RemoveAsync($"FoodRecords_{userId}");
        }

        public async Task DeleteFoodRecord(int id, int userId)
        {
            await _context.FoodRecords.Where(r => r.Id == id && r.UserId == userId).ExecuteDeleteAsync();
            _logger.LogInformation("FoodRecord was deleted.");

            // Invalidate cache
            await _cache.RemoveAsync($"FoodRecord_{userId}_{id}");
            await _cache.RemoveAsync($"FoodRecords_{userId}");
        }

        public async Task<FoodRecord[]> GetAllFoodRecords(int userId, bool ascendingOrder = false)
        {
            var cacheKey = $"FoodRecords_{userId}";
            var cachedRecords = await _cache.GetStringAsync(cacheKey);
            if (cachedRecords != null)
            {
                return JsonSerializer.Deserialize<FoodRecord[]>(cachedRecords);
            }

            FoodRecord[] records = null;

            if (!ascendingOrder)
            {
                records = await _context.FoodRecords.AsNoTracking()
                    .Where(record => record.UserId == userId)
                    .OrderByDescending(record => record.ConsumptionDate)
                    .ToArrayAsync();
            }
            else
            {
                records = await _context.FoodRecords.AsNoTracking()
                     .Where(record => record.UserId == userId)
                    .OrderBy(record => record.ConsumptionDate)
                    .ToArrayAsync();
            }

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(records));

            return records;
        }

        public async Task<FoodRecord> GetFoodRecord(int id, int userId)
        {
            var cacheKey = $"FoodRecord_{userId}_{id}";
            var cachedRecord = await _cache.GetStringAsync(cacheKey);
            if (cachedRecord != null)
            {
                return JsonSerializer.Deserialize<FoodRecord>(cachedRecord);
            }

            var record = await _context.FoodRecords.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (record != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(record));
            }

            return record;
        }

        public async Task UpdateFoodRecord(int id, int userId, FoodRecordDTO record)
        {
            if (!ValidationHelper.TryValidateObject(record, out var validationResults))
            {
                throw new CustomValidationException(validationResults);
            }

            var userExists = await UserExists(userId);
            var foodRecord = await _context.FoodRecords.Where(t => t.UserId == userId && t.Id == id)
                  .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.ConsumptionDate, record.ConsumptionDate)
                .SetProperty(b => b.Quantity, record.Quantity)
                .SetProperty(b => b.FoodItemId, record.FoodItemId));

            _logger.LogInformation("FoodRecord was updated.");

            // Invalidate cache
            await _cache.RemoveAsync($"FoodRecord_{userId}_{id}");
            await _cache.RemoveAsync($"FoodRecords_{userId}");
        }

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
