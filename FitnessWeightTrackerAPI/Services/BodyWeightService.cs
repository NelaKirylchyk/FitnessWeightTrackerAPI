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
    public class BodyWeightService : IBodyWeightService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<BodyWeightService> _logger;

        public BodyWeightService(FitnessWeightTrackerDbContext context, ILogger<BodyWeightService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task DeleteAllBodyweightRecords(int userId)
        {
            await _context.BodyWeightRecords.Where(x => x.UserId == userId).ExecuteDeleteAsync();
            _logger.LogInformation("All BodyWeightRecords were deleted.");
            await _cache.RemoveAsync($"BodyWeightRecords_{userId}");
        }

        public async Task DeleteBodyweightRecord(int id, int userId)
        {
            await _context.BodyWeightRecords.Where(r => r.Id == id && r.UserId == userId).ExecuteDeleteAsync();
            _logger.LogInformation("BodyWeightRecord was deleted.");
            await _cache.RemoveAsync($"BodyWeightRecord_{userId}_{id}");
            await _cache.RemoveAsync($"BodyWeightRecords_{userId}");
        }

        public async Task<BodyWeightRecord> GetBodyweightRecord(int id, int userId)
        {
            var cacheKey = $"BodyWeightRecord_{userId}_{id}";
            var cachedRecord = await _cache.GetStringAsync(cacheKey);
            if (cachedRecord != null)
            {
                return JsonSerializer.Deserialize<BodyWeightRecord>(cachedRecord);
            }

            var record = await _context.BodyWeightRecords.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (record != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(record));
            }

            return record;
        }

        public async Task<BodyWeightRecord[]> GetAllUserBodyweightRecords(int userId, bool ascendingOrder = false)
        {
            var cacheKey = $"BodyWeightRecords_{userId}";
            var cachedRecords = await _cache.GetStringAsync(cacheKey);
            if (cachedRecords != null)
            {
                return JsonSerializer.Deserialize<BodyWeightRecord[]>(cachedRecords);
            }

            BodyWeightRecord[] records = null;

            if (!ascendingOrder)
            {
                records = await _context.BodyWeightRecords.AsNoTracking()
                    .Where(record => record.UserId == userId)
                    .OrderByDescending(record => record.Date)
                    .ToArrayAsync();
            }
            else
            {
                records = await _context.BodyWeightRecords.AsNoTracking()
                    .Where(record => record.UserId == userId)
                    .OrderBy(record => record.Date)
                    .ToArrayAsync();
            }

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(records));
            return records;
        }

        public async Task UpdateBodyweightRecord(int id, int userId, BodyWeightRecordDTO record)
        {
            if (!ValidationHelper.TryValidateObject(record, out var validationResults))
            {
                _logger.LogError("Validation error occured.");
                throw new CustomValidationException(validationResults);
            }

            await _context.BodyWeightRecords.Where(t => t.UserId == userId && t.Id == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Weight, record.Weight)
                .SetProperty(b => b.Date, record.Date));

            _logger.LogInformation("BodyWeightRecord was updated.");

            await _cache.RemoveAsync($"BodyWeightRecord_{userId}_{id}");
            await _cache.RemoveAsync($"BodyWeightRecords_{userId}");
        }

        public async Task<BodyWeightRecord> AddBodyweightRecord(int userId, BodyWeightRecordDTO record)
        {
            if (record == null)
                throw new ArgumentNullException(nameof(record));

            var userExists = await UserExists(userId);
            BodyWeightRecord? entity = null;

            if (userExists)
            {
                entity = new BodyWeightRecord()
                {
                    UserId = userId,
                    Date = record.Date,
                    Weight = record.Weight
                };

                // Validate Entity
                if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.BodyWeightRecords.Add(entity);
                await _context.SaveChangesAsync();

                _logger.LogInformation("BodyWeightRecord was created.");
                await _cache.RemoveAsync($"BodyWeightRecords_{userId}");
            }

            return entity;
        }

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
