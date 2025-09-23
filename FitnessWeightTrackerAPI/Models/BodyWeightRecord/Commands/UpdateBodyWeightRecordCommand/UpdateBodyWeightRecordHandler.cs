using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.UpdateBodyWeightRecordCommand
{
    public class UpdateBodyWeightRecordHandler : IRequestHandler<UpdateBodyWeightRecordCommand, bool>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<UpdateBodyWeightRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public UpdateBodyWeightRecordHandler(
            FitnessWeightTrackerDbContext context,
            ILogger<UpdateBodyWeightRecordHandler> logger,
            IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<bool> Handle(UpdateBodyWeightRecordCommand request, CancellationToken cancellationToken)
        {
            // Validate input DTO (has DataAnnotations)
            if (!ValidationHelper.TryValidateObject(request.Record, out var validationResults))
            {
                _logger.LogError("Validation error while updating BodyWeightRecord.");
                throw new CustomValidationException(validationResults);
            }

            var record = await _context.BodyWeightRecords
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.UserId == request.UserId, cancellationToken);

            if (record == null)
            {
                _logger.LogInformation("BodyWeightRecord not found for update. Id={Id}, UserId={UserId}", request.Id, request.UserId);
                return false;
            }

            // Update fields
            record.Weight = request.Record.Weight;
            record.Date = request.Record.Date;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BodyWeightRecord was updated. Id={Id}, UserId={UserId}", request.Id, request.UserId);

            // Invalidate caches so subsequent GETs see fresh data
            await _cache.RemoveAsync($"BodyWeightRecord_{request.UserId}_{request.Id}", cancellationToken);
            await _cache.RemoveAsync($"BodyWeightRecords_{request.UserId}", cancellationToken);

            return true;
        }
    }
}
