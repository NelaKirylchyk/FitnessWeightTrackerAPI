using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Commands.CreateFoodItemCommand
{
    public class AddFoodItemHandler : IRequestHandler<AddFoodItemCommand, FoodItem>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<AddFoodItemHandler> _logger;
        private readonly IDistributedCache _cache;

        public AddFoodItemHandler(FitnessWeightTrackerDbContext context, ILogger<AddFoodItemHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<FoodItem> Handle(AddFoodItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new FoodItem
            {
                Calories = request.Calories,
                Fat = request.Fat,
                Carbohydrates = request.Carbohydrates,
                Name = request.Name,
                Protein = request.Protein,
                ServingSize = request.ServingSize
            };

            if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
            {
                _logger.LogError("Validation error while adding FoodItem.");
                throw new CustomValidationException(validationResults);
            }

            _context.FoodItems.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("FoodItem was added.");
            await _cache.RemoveAsync("AllFoodItems");

            return entity;
        }
    }
}
