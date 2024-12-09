using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Queries.GetBodyWeightTargetQuery
{
    public class GetBodyWeightTargetHandler : IRequestHandler<GetBodyWeightTargetQuery, BodyWeightTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;

        public GetBodyWeightTargetHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<BodyWeightTarget> Handle(GetBodyWeightTargetQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"BodyWeightTarget_{request.UserId}";
            var cachedTarget = await _cache.GetStringAsync(cacheKey);
            if (cachedTarget != null)
            {
                return JsonSerializer.Deserialize<BodyWeightTarget>(cachedTarget);
            }

            var target = await _context.BodyWeightTargets.AsNoTracking()
                .FirstOrDefaultAsync(t => t.UserId == request.UserId, cancellationToken);
            if (target != null)
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(target));
            }

            return target;
        }
    }
}
