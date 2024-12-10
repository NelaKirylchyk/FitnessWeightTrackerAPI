using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Queries.GetNutritionTargetByIdQuery
{
    public class GetNutritionTargetByIdHandler : IRequestHandler<GetNutritionTargetByIdQuery, NutritionTarget>
    {
        private readonly FitnessWeightTrackerDbContext _context;

        public GetNutritionTargetByIdHandler(FitnessWeightTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<NutritionTarget> Handle(GetNutritionTargetByIdQuery request, CancellationToken cancellationToken)
        {
            var target = await _context.NutritionTargets
                .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == request.UserId, cancellationToken);

            return target;
        }
    }
}
