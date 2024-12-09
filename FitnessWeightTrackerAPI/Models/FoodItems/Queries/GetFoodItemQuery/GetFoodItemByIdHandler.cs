using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Queries.GetFoodItemQuery
{
    public class GetFoodItemByIdHandler : IRequestHandler<GetFoodItemByIdQuery, FoodItem>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetFoodItemByIdHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<FoodItem> Handle(GetFoodItemByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"FoodItem_{request.Id}";
            var cachedItem = await _cache.GetStringAsync(cacheKey);
            if (cachedItem != null)
            {
                return JsonSerializer.Deserialize<FoodItem>(cachedItem);
            }

            var foodItem = await _context.FoodItems.AsNoTracking().FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (foodItem != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(foodItem));
            }

            return foodItem;
        }
    }
}
