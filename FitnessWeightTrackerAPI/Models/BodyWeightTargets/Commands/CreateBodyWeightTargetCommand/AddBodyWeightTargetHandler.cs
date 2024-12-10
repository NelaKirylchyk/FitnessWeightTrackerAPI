using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.CreateBodyWeightTargetCommand
{
    public class AddBodyWeightTargetHandler : IRequestHandler<AddBodyWeightTargetCommand, BodyWeightTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<AddBodyWeightTargetHandler> _logger;
        private readonly IDistributedCache _cache;

        public AddBodyWeightTargetHandler(FitnessWeightTrackerDbContext context, ILogger<AddBodyWeightTargetHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<BodyWeightTarget> Handle(AddBodyWeightTargetCommand request, CancellationToken cancellationToken)
        {
            var anyBodyWeightTargetExists = await _context.BodyWeightTargets.AnyAsync(t => t.UserId == request.UserId, cancellationToken);

            if (anyBodyWeightTargetExists)
            {
                _logger.LogError("User already has bodyweight target.");
                throw new TargetAlreadyExistsException("User already has a bodyweight target.");
            }

            var entity = new BodyWeightTarget
            {
                UserId = request.UserId,
                TargetDate = request.Target.TargetDate,
                TargetWeight = request.Target.TargetWeight
            };

            if (!ValidationHelper.TryValidateObject(entity, out var validationResults))
            {
                _logger.LogError("Validation error while adding BodyWeightTarget.");
                throw new CustomValidationException(validationResults);
            }

            await _context.BodyWeightTargets.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("BodyWeightTarget was added.");
            await _cache.RemoveAsync($"BodyWeightTarget_{request.UserId}", cancellationToken);

            return entity;
        }
    }
}
