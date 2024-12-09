using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Queries.GetAllFoodItemsQuery
{
    public class GetAllFoodItemsHandler : IRequestHandler<GetAllFoodItemsQuery, IEnumerable<FoodItem>>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetAllFoodItemsHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<IEnumerable<FoodItem>> Handle(GetAllFoodItemsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "AllFoodItems";
            var cachedItems = await _cache.GetStringAsync(cacheKey);
            if (cachedItems != null)
            {
                return JsonSerializer.Deserialize<FoodItem[]>(cachedItems);
            }

            var foodItems = await _context.FoodItems.AsNoTracking().ToListAsync(cancellationToken);
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(foodItems));

            return foodItems;
        }
    }
}
