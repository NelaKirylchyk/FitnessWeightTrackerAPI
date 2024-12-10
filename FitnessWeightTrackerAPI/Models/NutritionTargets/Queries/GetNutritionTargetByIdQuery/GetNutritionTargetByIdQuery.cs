using MediatR;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Queries.GetNutritionTargetByIdQuery
{
    public class GetNutritionTargetByIdQuery : IRequest<NutritionTarget>
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
