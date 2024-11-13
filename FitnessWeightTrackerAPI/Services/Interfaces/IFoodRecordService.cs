using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;

namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    public interface IFoodRecordService
    {
        public Task<FoodRecord> GetFoodRecord(int id, int userId);

        public Task<FoodRecord[]> GetAllFoodRecords(int userId, bool ascendingOrder = false);

        public Task<FoodRecord> AddFoodRecord(FoodRecordDTO foodRecord, int userId);

        public Task DeleteAllFoodRecords(int userId);

        public Task DeleteFoodRecord(int id, int userId);

        public Task UpdateFoodRecord(int id, int userId, FoodRecordDTO record);
    }
}
