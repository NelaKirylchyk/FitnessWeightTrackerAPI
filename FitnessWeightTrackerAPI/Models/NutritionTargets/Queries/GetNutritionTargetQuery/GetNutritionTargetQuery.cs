using MediatR;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Queries.GetNutritionTargetQuery
{
    public class GetNutritionTargetQuery : IRequest<NutritionTarget>
    {
        public int UserId { get; set; }
    }
}
