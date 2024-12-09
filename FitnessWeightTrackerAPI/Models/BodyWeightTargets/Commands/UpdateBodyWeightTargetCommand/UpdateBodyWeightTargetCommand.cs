using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightTargets.Commands.UpdateBodyWeightTargetCommand
{
    public class UpdateBodyWeightTargetCommand : IRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public BodyWeightTargetDTO Target { get; set; }
    }
}
