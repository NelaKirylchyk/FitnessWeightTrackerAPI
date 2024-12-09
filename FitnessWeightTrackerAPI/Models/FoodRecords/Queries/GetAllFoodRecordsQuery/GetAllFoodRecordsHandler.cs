using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Queries.GetAllFoodRecordsQuery
{
    public class GetAllFoodRecordsHandler : IRequestHandler<GetAllFoodRecordsQuery, FoodRecord[]>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetAllFoodRecordsHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<FoodRecord[]> Handle(GetAllFoodRecordsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"FoodRecords_{request.UserId}";
            var cachedRecords = await _cache.GetStringAsync(cacheKey);
            if (cachedRecords != null)
            {
                return JsonSerializer.Deserialize<FoodRecord[]>(cachedRecords);
            }

            var records = request.AscendingOrder ?
                await _context.FoodRecords.AsNoTracking()
                    .Where(record => record.UserId == request.UserId)
                    .OrderBy(record => record.ConsumptionDate)
                    .ToArrayAsync(cancellationToken) :
                await _context.FoodRecords.AsNoTracking()
                    .Where(record => record.UserId == request.UserId)
                    .OrderByDescending(record => record.ConsumptionDate)
                    .ToArrayAsync(cancellationToken);

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(records));

            return records;
        }
    }

}
