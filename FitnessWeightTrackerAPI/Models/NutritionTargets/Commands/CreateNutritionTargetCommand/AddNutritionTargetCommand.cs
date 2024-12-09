using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.NutritionTargets.Commands.CreateNutritionTargetCommand
{
    public class AddNutritionTargetCommand : IRequest<NutritionTarget>
    {
        public int UserId { get; set; }

        public NutritionTargetDTO Target { get; set; }
    }
}
