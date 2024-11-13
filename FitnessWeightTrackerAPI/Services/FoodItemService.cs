using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Data.DTO;
using FitnessWeightTrackerAPI.Models;
using FitnessWeightTrackerAPI.Services.Helpers;
using FitnessWeightTrackerAPI.Services.Interfaces;
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
            await _context.FoodItems.ExecuteDeleteAsync();
        }

        public async Task DeleteFoodItem(int id)
        {
            await _context.FoodItems.Where(r => r.Id == id).ExecuteDeleteAsync();
        }

        public async Task<FoodItem[]> GetAllFoodItems()
        {
            FoodItem[] foodItems = await _context.FoodItems.AsNoTracking().ToArrayAsync();
            return foodItems;
        }

        public async Task<FoodItem> GetFoodItem(int id)
        {
            var foodItem = await _context.FoodItems.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
            return foodItem;
        }

        public async Task UpdateFoodItem(int id, FoodItemDTO record)
        {
            if (!ValidationHelper.TryValidateObject(record, out var validationResults))
            {
                throw new CustomValidationException(validationResults);
            }

            await _context.FoodItems.Where(t => t.Id == id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Protein, record.Protein)
                .SetProperty(b => b.Name, record.Name)
                .SetProperty(b => b.Calories, record.Calories)
                .SetProperty(b => b.Carbohydrates, record.Carbohydrates)
                .SetProperty(b => b.Fat, record.Fat)
                .SetProperty(b => b.ServingSize, record.ServingSize));
        }
    }
}
