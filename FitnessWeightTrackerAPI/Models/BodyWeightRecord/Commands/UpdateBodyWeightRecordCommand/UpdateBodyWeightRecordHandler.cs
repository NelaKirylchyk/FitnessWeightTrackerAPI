using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.CreateBodyWeightRecordCommand;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.UpdateBodyWeightRecordCommand
{

    public class UpdateBodyWeightRecordHandler : IRequestHandler<UpdateBodyWeightRecordCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<AddBodyWeightRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public UpdateBodyWeightRecordHandler(
            FitnessWeightTrackerDbContext context,
            ILogger<AddBodyWeightRecordHandler> logger,
            IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(UpdateBodyWeightRecordCommand request, CancellationToken cancellationToken)
        {
            if (!ValidationHelper.TryValidateObject(request.Record, out var validationResults))
            {
                _logger.LogError("Validation error while updating BodyWeightRecord.");
                throw new CustomValidationException(validationResults);
            }

            await _context.BodyWeightRecords.Where(t => t.UserId == request.UserId && t.Id == request.Id)
               .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Weight, request.Record.Weight)
               .SetProperty(b => b.Date, request.Record.Date));

            _logger.LogInformation("BodyWeightRecord was updated.");
            await _cache.RemoveAsync($"BodyWeightRecord_{request.UserId}_{request.Id}", cancellationToken);
            await _cache.RemoveAsync($"BodyWeightRecords_{request.UserId}", cancellationToken);

            return Unit.Value;
        }
    }
}
