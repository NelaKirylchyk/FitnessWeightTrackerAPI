using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.CreateNutritionTargetCommand
{
    public class AddNutritionTargetHandler : IRequestHandler<AddNutritionTargetCommand, NutritionTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<AddNutritionTargetHandler> _logger;
        private readonly IDistributedCache _cache;

        public AddNutritionTargetHandler(FitnessWeightTrackerDbContext context, ILogger<AddNutritionTargetHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<NutritionTarget> Handle(AddNutritionTargetCommand request, CancellationToken cancellationToken)
        {
            var anyNutriotionTargetExists = await _context.NutritionTargets.AnyAsync(t => t.UserId == request.UserId, cancellationToken);

            if (anyNutriotionTargetExists)
            {
                _logger.LogError("User already has nutrition target.");
                throw new NutritionTargetAlreadyExistsException("User already has a nutrition target.");
            }

            var entity = new NutritionTarget
            {
                UserId = request.UserId,
                DailyCalories = request.Target.DailyCalories,
                DailyCarbonohydrates = request.Target.DailyCarbonohydrates,
                DailyFat = request.Target.DailyFat,
                DailyProtein = request.Target.DailyProtein
            };

            if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
            {
                _logger.LogError("Validation error while adding NutritionTarget.");
                throw new CustomValidationException(validationResults);
            }

            await _context.NutritionTargets.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("NutritionTarget was added.");
            await _cache.RemoveAsync($"NutritionTarget_{request.UserId}", cancellationToken);

            return entity;
        }
    }
}
