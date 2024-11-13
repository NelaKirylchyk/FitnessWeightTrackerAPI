using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;

namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    public interface IBodyWeightService
    {
        public Task<BodyWeightRecord> GetBodyweightRecord(int id, int userId);

        public Task<BodyWeightRecord[]> GetAllUserBodyweightRecords(int userId, bool ascendingOrder = false);

        public Task<BodyWeightRecord> AddBodyweightRecord(int userId, BodyWeightRecordDTO record);

        public Task DeleteAllBodyweightRecords(int userId);

        public Task DeleteBodyweightRecord(int id, int userId);

        public Task UpdateBodyweightRecord(int id, int userId, BodyWeightRecordDTO record);
    }
}
