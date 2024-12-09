using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Queries.GetBodyWeightTargetQuery
{
    public class GetBodyWeightTargetQuery : IRequest<BodyWeightTarget>
    {
        public int UserId { get; set; }
    }
}
