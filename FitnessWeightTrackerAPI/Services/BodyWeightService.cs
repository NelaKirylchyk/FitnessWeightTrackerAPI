using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System;
using FitnessWeightTrackerAPI.CustomExceptions;

namespace FitnessWeightTrackerAPI.Services
{
    public class BodyWeightService : IBodyWeightService
    {
        private readonly FitnessWeightTrackerDbContext _context;

        public BodyWeightService(FitnessWeightTrackerDbContext context)
        {
            _context = context;
        }

        #region BodyWeightRecords

        public async Task DeleteAllBodyweightRecords(int userId)
        {
            BodyWeightRecord[] existingRecords = await _context.BodyWeightRecords.Where(x => x.UserId == userId).ToArrayAsync();
            _context.BodyWeightRecords.RemoveRange(existingRecords);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteBodyweightRecord(int id, int userId)
        {
            var existingRecord = await _context.BodyWeightRecords.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (existingRecord != null)
            {
                _context.BodyWeightRecords.Remove(existingRecord);
                await _context.SaveChangesAsync();
            }
            return existingRecord != null;
        }

        public async Task<BodyWeightRecord> GetBodyweightRecord(int id, int userId)
        {
            var record = await _context.BodyWeightRecords.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            return record;
        }

        public async Task<BodyWeightRecord[]> GetAllUserBodyweightRecords(int userId, bool AscendingOrder = false)
        {
            BodyWeightRecord[] records = null;

            if (AscendingOrder == false)
            {
                records = await _context.BodyWeightRecords
                    .Where(record => record.UserId == userId)
                    .OrderByDescending(record => record.Date)
                    .ToArrayAsync();
            }
            else
            {
                records = await _context.BodyWeightRecords
                     .Where(record => record.UserId == userId)
                    .OrderBy(record => record.Date)
                    .ToArrayAsync();
            }

            return records;
        }

        public async Task<BodyWeightRecord> UpdateBodyweightRecord(int id, int userId, BodyWeightRecordDTO record)
        {
            var userExists = await UserExists(userId);
            var existingBodyWeightRecord = await _context.BodyWeightRecords.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);


            if (userExists && existingBodyWeightRecord != null)
            {
                existingBodyWeightRecord.Weight = record.Weight;
                existingBodyWeightRecord.Date = record.Date;
               
                // Validate updated entity
                if (!ValidationHelper.TryValidateObject(existingBodyWeightRecord, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.BodyWeightRecords.Update(existingBodyWeightRecord);
                await _context.SaveChangesAsync();
            }
            return existingBodyWeightRecord;
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
            }

            return entity;
        }

        #endregion


        #region BodyWeightTarget
        public async Task<BodyWeightTarget> GetUserBodyweightTarget(int userId)
        {
            var result = await _context.BodyWeightTargets.FirstOrDefaultAsync(target => target.UserId == userId);
            return result;
        }

        public async Task<BodyWeightTarget> AddBodyweightTarget(int userId, BodyWeightTargetDTO targetWeight)
        {
            var userExists = await UserExists(userId);
            var anyUserTargetExists = await _context.BodyWeightTargets.AnyAsync(t => t.UserId == userId);

            BodyWeightTarget? entity = null;

            if (userExists && !anyUserTargetExists)
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
            }
            return entity;
        }

        public async Task<BodyWeightTarget> UpdateBodyweightTarget(int id, int userId, BodyWeightTargetDTO targetWeight)
        {
            var userExists = await UserExists(userId);
            var existingBodyWeightTarget = await _context.BodyWeightTargets.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);


            if (userExists && existingBodyWeightTarget != null)
            {
                existingBodyWeightTarget.TargetWeight = targetWeight.TargetWeight;
                existingBodyWeightTarget.TargetDate = targetWeight.TargetDate;

                // Validate updated entity
                if (!ValidationHelper.TryValidateObject(existingBodyWeightTarget, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.BodyWeightTargets.Update(existingBodyWeightTarget);
                await _context.SaveChangesAsync();
            }
            return existingBodyWeightTarget;
        }

        public async Task<bool> DeleteBodyweightTarget(int id, int userId)
        {
            var target = await _context.BodyWeightTargets.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);
            if (target != null)
            {
                _context.BodyWeightTargets.Remove(target);
                await _context.SaveChangesAsync();
            }
            return target != null;
        }


        #endregion

        private async Task<bool> UserExists(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(r => r.Id == userId);
            return user != null;
        }
    }
}
