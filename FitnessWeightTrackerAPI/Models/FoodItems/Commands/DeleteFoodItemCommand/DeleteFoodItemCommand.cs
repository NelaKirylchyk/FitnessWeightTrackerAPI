using MediatR;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Commands.DeleteFoodItemCommand
{
    public class DeleteFoodItemCommand : IRequest
    {
        public int Id { get; set; }
    }
}
