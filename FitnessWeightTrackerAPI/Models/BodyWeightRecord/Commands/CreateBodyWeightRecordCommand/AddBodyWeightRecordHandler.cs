using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.CreateBodyWeightRecordCommand
{
    public class AddBodyWeightRecordHandler : IRequestHandler<AddBodyWeightRecordCommand, BodyWeightRecord>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<AddBodyWeightRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public AddBodyWeightRecordHandler(
            FitnessWeightTrackerDbContext context,
            ILogger<AddBodyWeightRecordHandler> logger,
            IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<BodyWeightRecord> Handle(AddBodyWeightRecordCommand request, CancellationToken cancellationToken)
        {
            var entity = new BodyWeightRecord
            {
                UserId = request.UserId,
                Date = request.Record.Date,
                Weight = request.Record.Weight
            };

            if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
            {
                _logger.LogError("Validation error while adding BodyWeightRecord.");
                throw new CustomValidationException(validationResults);
            }

            await _context.BodyWeightRecords.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BodyWeightRecord was created.");
            await _cache.RemoveAsync($"BodyWeightRecords_{request.UserId}", cancellationToken);

            return entity;
        }
    }
}
