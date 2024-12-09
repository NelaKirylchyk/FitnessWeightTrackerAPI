using MediatR;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Queries.GetAllFoodItemsQuery
{
    public class GetAllFoodItemsQuery : IRequest<IEnumerable<FoodItem>>
    {
    }
}
