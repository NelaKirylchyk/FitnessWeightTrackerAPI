using FitnessWeightTrackerAPI.CustomExceptions;
using FitnessWeightTrackerAPI.Data;
using FitnessWeightTrackerAPI.Services.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.UpdateBodyWeightTargetCommand
{
    public class UpdateBodyWeightTargetHandler : IRequestHandler<UpdateBodyWeightTargetCommand>
    {
        private readonly FitnessWeightTrackerDbContext _context;
        private readonly ILogger<UpdateBodyWeightTargetHandler> _logger;
        private readonly IDistributedCache _cache;

        public UpdateBodyWeightTargetHandler(FitnessWeightTrackerDbContext context, ILogger<UpdateBodyWeightTargetHandler> logger, IDistributedCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task<Unit> Handle(UpdateBodyWeightTargetCommand request, CancellationToken cancellationToken)
        {
            if (!ValidationHelper.TryValidateObject(request.Target, out var validationResults))
            {
                _logger.LogError("Validation error while updating BodyWeightTarget.");
                throw new CustomValidationException(validationResults);
            }

            var updatedRows = await _context.BodyWeightTargets
                .Where(t => t.UserId == request.UserId && t.Id == request.Id)
                .ExecuteUpdateAsync(setters => setters.SetProperty(b => b.TargetWeight, request.Target.TargetWeight).SetProperty(b => b.TargetDate, request.Target.TargetDate), cancellationToken);

            if (updatedRows == 0) // if no records were updated
            {
                throw new Exception("BodyWeightTarget not found");
            }

            _logger.LogInformation("BodyWeightTarget was updated.");
            await _cache.RemoveAsync($"BodyWeightTarget_{request.UserId}", cancellationToken);

            return Unit.Value;
        }
    }
}
