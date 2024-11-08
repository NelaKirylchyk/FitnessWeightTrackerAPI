namespace FitnessWeightTrackerAPI.Services.Interfaces
{
    using FitnessWeightTrackerAPI.Data.DTO;
    using FitnessWeightTrackerAPI.Models;

    public interface IFoodItemService
    {
        public Task<FoodItem> GetFoodItem(int id);

        public Task<FoodItem[]> GetAllFoodItems();

        public Task<FoodItem> AddFoodItem(FoodItemDTO foodItem);

        public Task DeleteAllFoodItems();

        public Task<bool> DeleteFoodItem(int id);

        public Task<FoodItem> UpdateFoodItem(int id, FoodItemDTO record);
    }
}
