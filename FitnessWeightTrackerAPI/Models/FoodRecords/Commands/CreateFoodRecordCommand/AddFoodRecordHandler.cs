using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Commands.CreateFoodRecordCommand
{
    public class AddFoodRecordHandler : IRequestHandler<AddFoodRecordCommand, FoodRecord>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<AddFoodRecordHandler> _logger;
        private readonly IDistributedCache _cache;

        public AddFoodRecordHandler(FitnessWeightTrackerDbContext context, ILogger<AddFoodRecordHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<FoodRecord> Handle(AddFoodRecordCommand request, CancellationToken cancellationToken)
        {
            var foodItemExists = _context.FoodItems.AsNoTracking().Any(x => x.Id == request.Record.FoodItemId);
            FoodRecord? entity = null;

            if (foodItemExists)
            {
                entity = new FoodRecord()
                {
                    UserId = request.UserId,
                    ConsumptionDate = request.Record.ConsumptionDate,
                    FoodItemId = request.Record.FoodItemId,
                    Quantity = request.Record.Quantity
                };

                // Validate  entity
                if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
                {
                    _logger.LogError("Validation error while adding FoodRecord.");
                    throw new CustomValidationException(validationResults);
                }

                await _context.FoodRecords.AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("FoodRecord was added.");
                await _cache.RemoveAsync($"FoodRecords_{request.UserId}", cancellationToken);
            }

            return entity;
        }
    }
}
