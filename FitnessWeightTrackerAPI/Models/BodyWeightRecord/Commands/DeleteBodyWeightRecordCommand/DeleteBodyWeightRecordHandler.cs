using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.DeleteBodyWeightRecordCommand
{
    public class DeleteBodyWeightRecordHandler : IRequestHandler<DeleteBodyWeightRecordCommand, bool>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<DeleteBodyWeightRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public DeleteBodyWeightRecordHandler(FitnessWeightTrackerDbContext context, ILogger<DeleteBodyWeightRecordHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<bool> Handle(DeleteBodyWeightRecordCommand request, CancellationToken cancellationToken)
        {
            var deletedRows = await _context.BodyWeightRecords
                .Where(r => r.Id == request.Id && r.UserId == request.UserId)
                .ExecuteDeleteAsync(cancellationToken);

            if (deletedRows == 0)
            {
                _logger.LogInformation("BodyWeightRecord not found for deletion.");
                return false;
            }

            _logger.LogInformation("BodyWeightRecord was deleted.");
            await _cache.RemoveAsync($"BodyWeightRecord_{request.UserId}_{request.Id}", cancellationToken);
            await _cache.RemoveAsync($"BodyWeightRecords_{request.UserId}", cancellationToken);

            return true;
        }
    }
}
