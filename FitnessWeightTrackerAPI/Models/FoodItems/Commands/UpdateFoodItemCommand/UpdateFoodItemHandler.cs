using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Commands.UpdateFoodItemCommand
{
    public class UpdateFoodItemHandler : IRequestHandler<UpdateFoodItemCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<UpdateFoodItemHandler> _logger;
        private readonly IDistributedCache _cache;

        public UpdateFoodItemHandler(FitnessWeightTrackerDbContext context, ILogger<UpdateFoodItemHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(UpdateFoodItemCommand request, CancellationToken cancellationToken)
        {
            var foodItem = await _context.FoodItems.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);
            if (foodItem == null)
            {
                throw new Exception($"FoodItem with Id {request.Id} not found.");
            }

            foodItem.Name = request.Name;
            foodItem.Calories = request.Calories;
            foodItem.Carbohydrates = request.Carbohydrates;
            foodItem.Protein = request.Protein;
            foodItem.Fat = request.Fat;
            foodItem.ServingSize = request.ServingSize;

            if (!ValidationHelper.TryValidateObject(foodItem, out var validationResults))
            {
                _logger.LogError("Validation error while updating FoodItem.");
                throw new CustomValidationException(validationResults);
            }


            _context.FoodItems.Update(foodItem);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("FoodItem was updated.");
            await _cache.RemoveAsync($"FoodItem_{request.Id}");
            await _cache.RemoveAsync("AllFoodItems");

            return Unit.Value;
        }
    }
}
