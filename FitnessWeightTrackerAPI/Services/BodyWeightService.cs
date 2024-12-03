﻿using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using FitnessWeightTrackerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Services
{
    public class BodyWeightService : IBodyWeightService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<BodyWeightService> _logger;

        public BodyWeightService(FitnessWeightTrackerDbContext context, ILogger<BodyWeightService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region BodyWeightRecords

        public async Task DeleteAllBodyweightRecords(int userId)
        {
            await _context.BodyWeightRecords.Where(x => x.UserId == userId).ExecuteDeleteAsync();
            _logger.LogInformation("All BodyWeightRecords were deleted.");
        }

        public async Task DeleteBodyweightRecord(int id, int userId)
        {
            await _context.BodyWeightRecords.Where(r => r.Id == id && r.UserId == userId).ExecuteDeleteAsync();
            _logger.LogInformation("BodyWeightRecord was deleted.");
        }

        public async Task<BodyWeightRecord> GetBodyweightRecord(int id, int userId)
        {
            var record = await _context.BodyWeightRecords.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            return record;
        }

        public async Task<BodyWeightRecord[]> GetAllUserBodyweightRecords(int userId, bool ascendingOrder = false)
        {
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
        }

        public async Task<BodyWeightRecord> AddBodyweightRecord(int userId, BodyWeightRecordDTO record)
        {
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
            }

            return entity;
        }

        #endregion


        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
