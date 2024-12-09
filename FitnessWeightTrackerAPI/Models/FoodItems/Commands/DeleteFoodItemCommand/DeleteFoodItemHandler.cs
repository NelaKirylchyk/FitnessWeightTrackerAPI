using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Commands.DeleteFoodItemCommand
{
    public class DeleteFoodItemHandler : IRequestHandler<DeleteFoodItemCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<DeleteFoodItemHandler> _logger;
        private readonly IDistributedCache _cache;

        public DeleteFoodItemHandler(FitnessWeightTrackerDbContext context, ILogger<DeleteFoodItemHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(DeleteFoodItemCommand request, CancellationToken cancellationToken)
        {
            var foodItem = await _context.FoodItems.FirstOrDefaultAsync(f => f.Id == request.Id, cancellationToken);
            if (foodItem == null)
            {
                throw new Exception($"FoodItem with Id {request.Id} not found.");
            }

            _context.FoodItems.Remove(foodItem);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("FoodItem was deleted.");
            await _cache.RemoveAsync($"FoodItem_{request.Id}");
            await _cache.RemoveAsync("AllFoodItems");

            return Unit.Value;
        }
    }
}
