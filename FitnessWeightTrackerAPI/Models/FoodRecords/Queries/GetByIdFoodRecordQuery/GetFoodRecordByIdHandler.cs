using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Queries.GetByIdFoodRecordQuery
{
    public class GetFoodRecordByIdHandler : IRequestHandler<GetFoodRecordByIdQuery, FoodRecord>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetFoodRecordByIdHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<FoodRecord> Handle(GetFoodRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"FoodRecord_{request.UserId}_{request.Id}";
            var cachedRecord = await _cache.GetStringAsync(cacheKey);
            if (cachedRecord != null)
            {
                return JsonSerializer.Deserialize<FoodRecord>(cachedRecord);
            }

            var record = await _context.FoodRecords.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.UserId == request.UserId, cancellationToken);
            if (record != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(record));
            }

            return record;
        }
    }
}
