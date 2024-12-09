using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.DeleteBodyWeightTargetCommand
{
    public class DeleteBodyWeightTargetCommand : IRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
