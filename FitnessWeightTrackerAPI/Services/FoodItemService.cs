using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Services
{
    public class FoodItemService : IFoodItemService
    {
        private readonly FitnessWeightTrackerDbContext _context;

        public FoodItemService(FitnessWeightTrackerDbContext context)
        {
            _context = context;
        }
        public async Task<FoodItem> AddFoodItem(FoodItemDTO foodItem)
        {
            var entity = new FoodItem()
            {
                Calories = foodItem.Calories,
                Fat = foodItem.Fat,
                Carbohydrates = foodItem.Carbohydrates,
                Name = foodItem.Name,
                Protein = foodItem.Protein,
                ServingSize = foodItem.ServingSize
            };

            // Validate Entity
            if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
            {
                throw new CustomValidationException(validationResults);
            }

            _context.FoodItems.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAllFoodItems()
        {
            var existingFoodItems = await _context.FoodItems.ToArrayAsync();
            _context.FoodItems.RemoveRange(existingFoodItems);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteFoodItem(int id)
        {
            var existingFoodItem = await _context.FoodItems.FirstOrDefaultAsync(r => r.Id == id);
            if (existingFoodItem != null)
            {
                _context.FoodItems.Remove(existingFoodItem);
                await _context.SaveChangesAsync();
            }
            return existingFoodItem != null;
        }

        public async Task<FoodItem[]> GetAllFoodItems()
        {
            FoodItem[] foodItems = await _context.FoodItems.ToArrayAsync();
            return foodItems;
        }

        public async Task<FoodItem> GetFoodItem(int id)
        {
            var foodItem = await _context.FoodItems.FirstOrDefaultAsync(r => r.Id == id);
            return foodItem;
        }

        public async Task<FoodItem> UpdateFoodItem(int id, FoodItemDTO record)
        {
            var foodItem = await _context.FoodItems.FirstOrDefaultAsync(t => t.Id == id);

            if (foodItem != null)
            {
                foodItem.Protein = record.Protein;
                foodItem.Name = record.Name;
                foodItem.Calories = record.Calories;
                foodItem.Carbohydrates = record.Carbohydrates;
                foodItem.Fat = record.Fat;
                foodItem.ServingSize = record.ServingSize;

                // Validate updated entity
                if (!ValidationHelper.TryValidateObject(foodItem, out var validationResults))
                {
                    throw new CustomValidationException(validationResults);
                }

                _context.FoodItems.Update(foodItem);
                await _context.SaveChangesAsync();
            }
            return foodItem;
        }
    }
}
