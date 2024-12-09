using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.UpdateNutritionTargetCommand
{
    public class UpdateNutritionTargetCommand : IRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public NutritionTargetDTO Target { get; set; }
    }
}
