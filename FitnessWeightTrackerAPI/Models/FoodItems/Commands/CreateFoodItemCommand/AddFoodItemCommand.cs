using MediatR;
namespace FitnessWeightTrackerAPI.Models.FoodItems.Commands.CreateFoodItemCommand
{
    public class AddFoodItemCommand : IRequest<FoodItem>
    {
        public string Name { get; set; }

        public int Calories { get; set; }

        public int Carbohydrates { get; set; }

        public int Protein { get; set; }

        public int Fat { get; set; }

        public int ServingSize { get; set; }
    }
}
