using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Commands.DeleteFoodRecordCommand
{
    public class DeleteFoodRecordHandler : IRequestHandler<DeleteFoodRecordCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<DeleteFoodRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public DeleteFoodRecordHandler(FitnessWeightTrackerDbContext context, ILogger<DeleteFoodRecordHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(DeleteFoodRecordCommand request, CancellationToken cancellationToken)
        {
            var deletedRows = await _context.FoodRecords
                .Where(t => t.UserId == request.UserId && t.Id == request.Id)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows == 0) // if no records were deleted
            {
                throw new Exception("FoodRecord not found");
            }

            _logger.LogInformation("FoodRecord was deleted.");
            await _cache.RemoveAsync($"FoodRecord_{request.UserId}_{request.Id}", cancellationToken);
            await _cache.RemoveAsync($"FoodRecords_{request.UserId}", cancellationToken);

            return Unit.Value;
        }
    }
}
