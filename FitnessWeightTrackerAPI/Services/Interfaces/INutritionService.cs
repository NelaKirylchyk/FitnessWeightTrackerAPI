using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;

namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    public interface INutritionService
    {
        public Task<FoodRecord> GetFoodRecord(int id, int userId);
        public Task<FoodRecord[]> GetAllFoodRecords(int userId, bool AscendingOrder = false);
        public Task<FoodRecord> AddFoodRecord(FoodRecordDTO foodRecord, int userId);
        public Task DeleteAllFoodRecords(int userId);
        public Task<bool> DeleteFoodRecord(int id, int userId);
        public Task<FoodRecord> UpdateFoodRecord(int id, int userId, FoodRecordDTO record);

        public Task<NutritionTarget> UpdateNutritionTarget(int id, int userId, NutritionTargetDTO targetWeight);

        public Task<NutritionTarget> GetNutritionTarget(int userId);
        public Task<NutritionTarget> AddNutritionTarget(int userId, NutritionTargetDTO target);
        public Task<bool> DeleteNutritionTarget(int id, int userId);
    }
}
