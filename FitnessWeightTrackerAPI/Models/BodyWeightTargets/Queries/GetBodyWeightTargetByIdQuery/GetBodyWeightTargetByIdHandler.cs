using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Queries.GetBodyWeightTargetByIdQuery
{
    public class GetBodyWeightTargetByIdHandler : IRequestHandler<GetBodyWeightTargetByIdQuery, BodyWeightTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetBodyWeightTargetByIdHandler> _logger;

        public GetBodyWeightTargetByIdHandler(FitnessWeightTrackerDbContext context, IDistributedCache cache, ILogger<GetBodyWeightTargetByIdHandler> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<BodyWeightTarget> Handle(GetBodyWeightTargetByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"BodyWeightTarget_{request.UserId}_{request.Id}";
            var cachedTarget = await _cache.GetStringAsync(cacheKey);

            if (cachedTarget != null)
            {
                return JsonSerializer.Deserialize<BodyWeightTarget>(cachedTarget);
            }

            var bodyWeightTarget = await _context.BodyWeightTargets
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == request.UserId, cancellationToken);

            if (bodyWeightTarget == null)
            {
                _logger.LogError($"BodyWeightTarget not found for user {request.UserId} and id {request.Id}");
            }
            else
            {
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(bodyWeightTarget));
            }

            return bodyWeightTarget;
        }
    }
}
