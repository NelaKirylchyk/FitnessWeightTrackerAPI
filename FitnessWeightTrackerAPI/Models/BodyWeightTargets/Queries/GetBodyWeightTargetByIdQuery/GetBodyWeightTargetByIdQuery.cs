using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Queries.GetBodyWeightTargetByIdQuery
{
    public class GetBodyWeightTargetByIdQuery : IRequest<BodyWeightTarget>
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
