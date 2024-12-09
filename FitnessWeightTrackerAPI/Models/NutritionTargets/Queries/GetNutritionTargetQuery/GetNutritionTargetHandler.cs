using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Queries.GetNutritionTargetQuery
{
    public class GetNutritionTargetHandler : IRequestHandler<GetNutritionTargetQuery, NutritionTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetNutritionTargetHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<NutritionTarget> Handle(GetNutritionTargetQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"NutritionTarget_{request.UserId}";
            var cachedRecord = await _cache.GetStringAsync(cacheKey);
            if (cachedRecord != null)
            {
                return JsonSerializer.Deserialize<NutritionTarget>(cachedRecord);
            }

            var record = await _context.NutritionTargets.AsNoTracking()
                .FirstOrDefaultAsync(r => r.UserId == request.UserId, cancellationToken);

            if (record != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(record));
            }

            return record;
        }
    }
}
