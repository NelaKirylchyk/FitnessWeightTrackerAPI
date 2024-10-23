using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    public interface IBodyWeightService
    {
        public Task<BodyWeightRecord> GetBodyweightRecord(int id, int userId);
        public Task<BodyWeightRecord[]> GetAllUserBodyweightRecords(int userId, bool AscendingOrder = false);
        public Task<BodyWeightRecord> AddBodyweightRecord(int userId, BodyWeightRecordDTO record);
        public Task DeleteAllBodyweightRecords(int userId);
        public Task<bool> DeleteBodyweightRecord(int id, int userId);
        public Task<BodyWeightRecord> UpdateBodyweightRecord(int id, int userId, BodyWeightRecordDTO record);

        public Task<BodyWeightTarget> UpdateBodyweightTarget(int id, int userId, BodyWeightTargetDTO targetWeight);

        public Task<BodyWeightTarget> GetUserBodyweightTarget(int userId);
        public Task<BodyWeightTarget> AddBodyweightTarget(int userId, BodyWeightTargetDTO Target);
        public Task<bool> DeleteBodyweightTarget(int id, int userId);


    }
}
