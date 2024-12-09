using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.DeleteNutritionTargetCommand
{
    public class DeleteNutritionTargetHandler : IRequestHandler<DeleteNutritionTargetCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<DeleteNutritionTargetHandler> _logger;
        private readonly IDistributedCache _cache;

        public DeleteNutritionTargetHandler(FitnessWeightTrackerDbContext context, ILogger<DeleteNutritionTargetHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(DeleteNutritionTargetCommand request, CancellationToken cancellationToken)
        {
            var deletedRows = await _context.NutritionTargets
                .Where(t => t.UserId == request.UserId && t.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows == 0) // if no records were deleted
            {
                throw new Exception("NutritionTarget not found");
            }

            _logger.LogInformation("NutritionTarget was deleted.");
            await _cache.RemoveAsync($"NutritionTarget_{request.UserId}", cancellationToken);

            return Unit.Value;
        }
    }
}
