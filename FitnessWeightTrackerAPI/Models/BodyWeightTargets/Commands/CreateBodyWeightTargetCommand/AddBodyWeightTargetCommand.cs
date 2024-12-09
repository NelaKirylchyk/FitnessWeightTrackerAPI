using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.CreateBodyWeightTargetCommand
{
    public class AddBodyWeightTargetCommand : IRequest<BodyWeightTarget>
    {
        public int UserId { get; set; }

        public BodyWeightTargetDTO Target { get; set; }
    }
}
