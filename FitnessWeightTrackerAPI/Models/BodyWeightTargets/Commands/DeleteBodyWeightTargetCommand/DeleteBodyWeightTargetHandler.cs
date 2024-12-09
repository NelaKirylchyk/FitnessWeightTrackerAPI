using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.DeleteBodyWeightTargetCommand
{
    public class DeleteBodyWeightTargetHandler : IRequestHandler<DeleteBodyWeightTargetCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<DeleteBodyWeightTargetHandler> _logger;
        private readonly IDistributedCache _cache;

        public DeleteBodyWeightTargetHandler(FitnessWeightTrackerDbContext context, ILogger<DeleteBodyWeightTargetHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(DeleteBodyWeightTargetCommand request, CancellationToken cancellationToken)
        {
            var deletedRows = await _context.BodyWeightTargets
                .Where(t => t.UserId == request.UserId && t.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows == 0) // if no records were deleted
            {
                throw new Exception("BodyWeightTarget not found");
            }

            _logger.LogInformation("BodyWeightTarget was deleted.");
            await _cache.RemoveAsync($"BodyWeightTarget_{request.UserId}", cancellationToken);

            return Unit.Value;
        }
    }
}
