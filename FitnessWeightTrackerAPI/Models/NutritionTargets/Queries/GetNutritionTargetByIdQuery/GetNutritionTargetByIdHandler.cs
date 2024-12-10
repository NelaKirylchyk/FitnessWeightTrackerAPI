using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Queries.GetNutritionTargetByIdQuery
{
    public class GetNutritionTargetByIdHandler : IRequestHandler<GetNutritionTargetByIdQuery, NutritionTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetNutritionTargetByIdHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<NutritionTarget> Handle(GetNutritionTargetByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"NutritionTarget_{request.UserId}_{request.Id}";
            var cachedTarget = await _cache.GetStringAsync(cacheKey);

            if (cachedTarget != null)
            {
                return JsonSerializer.Deserialize<NutritionTarget>(cachedTarget);
            }

            var target = await _context.NutritionTargets
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == request.UserId, cancellationToken);

            if (target != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(target));
            }

            return target;
        }
    }
}
