using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Queries.GetBodyWeightTargetByIdQuery
{
    public class GetBodyWeightTargetByIdHandler : IRequestHandler<GetBodyWeightTargetByIdQuery, BodyWeightTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<GetBodyWeightTargetByIdHandler> _logger;

        public GetBodyWeightTargetByIdHandler(FitnessWeightTrackerDbContext context, ILogger<GetBodyWeightTargetByIdHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<BodyWeightTarget> Handle(GetBodyWeightTargetByIdQuery request, CancellationToken cancellationToken)
        {
            var bodyWeightTarget = await _context.BodyWeightTargets
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == request.UserId, cancellationToken);

            if (bodyWeightTarget == null)
            {
                _logger.LogError($"BodyWeightTarget not found for user {request.UserId} and id {request.Id}");
            }

            return bodyWeightTarget;
        }
    }
}
