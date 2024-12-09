using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Commands.UpdateFoodRecordCommand
{
    public class UpdateFoodRecordHandler : IRequestHandler<UpdateFoodRecordCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<UpdateFoodRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public UpdateFoodRecordHandler(FitnessWeightTrackerDbContext context, ILogger<UpdateFoodRecordHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(UpdateFoodRecordCommand request, CancellationToken cancellationToken)
        {
            if (!ValidationHelper.TryValidateObject(request.Record, out var validationResults))
            {
                _logger.LogError("Validation error while updating FoodRecord.");
                throw new CustomValidationException(validationResults);
            }

            var updatedRows = await _context.FoodRecords
                .Where(t => t.UserId == request.UserId && t.Id == request.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(b => b.ConsumptionDate, request.Record.ConsumptionDate)
                    .SetProperty(b => b.Quantity, request.Record.Quantity)
                    .SetProperty(b => b.FoodItemId, request.Record.FoodItemId), cancellationToken);

            if (updatedRows == 0) // if no records were updated
            {
                throw new Exception("FoodRecord not found");
            }

            _logger.LogInformation("FoodRecord was updated.");
            await _cache.RemoveAsync($"FoodRecord_{request.UserId}_{request.Id}", cancellationToken);
            await _cache.RemoveAsync($"FoodRecords_{request.UserId}", cancellationToken);

            return Unit.Value;
        }
    }

}
