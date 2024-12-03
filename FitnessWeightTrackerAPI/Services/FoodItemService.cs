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
    public class FoodItemService : IFoodItemService
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<FoodItemService> _logger;
        private readonly IDistributedCache _cache;

        public FoodItemService(FitnessWeightTrackerDbContext context, ILogger<FoodItemService> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
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
                _logger.LogError("Validation error while adding FoodItem.");
                throw new CustomValidationException(validationResults);
            }

            _context.FoodItems.Add(entity);
            await _context.SaveChangesAsync();
            _logger.LogInformation("FoodItem was added.");

            // Invalidate cache
            await _cache.RemoveAsync("AllFoodItems");

            return entity;
        }

        public async Task DeleteAllFoodItems()
        {
            await _context.FoodItems.ExecuteDeleteAsync();
            _logger.LogInformation("All FoodItems were deleted.");

            // Invalidate cache
            await _cache.RemoveAsync("AllFoodItems");
        }

        public async Task DeleteFoodItem(int id)
        {
            await _context.FoodItems.Where(r => r.Id == id).ExecuteDeleteAsync();
            _logger.LogInformation("FoodItem was added.");

            // Invalidate cache
            await _cache.RemoveAsync($"FoodItem_{id}");
            await _cache.RemoveAsync("AllFoodItems");
        }

        public async Task<FoodItem[]> GetAllFoodItems()
        {
            var cacheKey = "AllFoodItems";
            var cachedItems = await _cache.GetStringAsync(cacheKey);
            if (cachedItems != null)
            {
                return JsonSerializer.Deserialize<FoodItem[]>(cachedItems);
            }

            FoodItem[] foodItems = await _context.FoodItems.AsNoTracking().ToArrayAsync();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(foodItems));

            return foodItems;
        }

        public async Task<FoodItem> GetFoodItem(int id)
        {
            var cacheKey = $"FoodItem_{id}";
            var cachedItem = await _cache.GetStringAsync(cacheKey);

            if (cachedItem != null)
            {
                return JsonSerializer.Deserialize<FoodItem>(cachedItem);
            }

            var foodItem = await _context.FoodItems.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

            if (foodItem != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(foodItem));
            }

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

            _logger.LogInformation("FoodItem was updated.");

            // Invalidate cache
            await _cache.RemoveAsync($"FoodItem_{id}");
            await _cache.RemoveAsync("AllFoodItems");
        }
    }
}
