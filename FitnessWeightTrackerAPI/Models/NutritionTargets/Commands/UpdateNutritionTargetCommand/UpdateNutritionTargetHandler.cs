using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.UpdateNutritionTargetCommand
{
    public class UpdateNutritionTargetHandler : IRequestHandler<UpdateNutritionTargetCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<UpdateNutritionTargetHandler> _logger;
        private readonly IDistributedCache _cache;

        public UpdateNutritionTargetHandler(FitnessWeightTrackerDbContext context, ILogger<UpdateNutritionTargetHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(UpdateNutritionTargetCommand request, CancellationToken cancellationToken)
        {
            if (!ValidationHelper.TryValidateObject(request.Target, out var validationResults))
            {
                _logger.LogError("Validation error while updating NutritionTarget.");
                throw new CustomValidationException(validationResults);
            }

            var updatedRows = await _context.NutritionTargets
                .Where(t => t.UserId == request.UserId && t.Id == request.Id)
                .ExecuteUpdateAsync(
                setters => setters.SetProperty(b => b.DailyProtein, request.Target.DailyProtein)
                .SetProperty(b => b.DailyFat, request.Target.DailyFat)
                .SetProperty(b => b.DailyCarbonohydrates, request.Target.DailyCarbonohydrates)
                .SetProperty(b => b.DailyCalories, request.Target.DailyCalories), cancellationToken);

            if (updatedRows == 0) // if no records were updated
            {
                throw new Exception("NutritionTarget not found");
            }

            _logger.LogInformation("NutritionTarget was updated.");
            await _cache.RemoveAsync($"NutritionTarget_{request.UserId}", cancellationToken);

            return Unit.Value;
        }
    }
}
