using MediatR;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.DeleteNutritionTargetCommand
{
    public class DeleteNutritionTargetCommand : IRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
