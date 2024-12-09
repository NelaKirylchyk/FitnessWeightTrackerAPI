using FitnessWeightTrackerAPI.Data;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace FitnessWeightTrackerAPI.Models.FoodItems.Queries.GetFoodItemQuery
{
    public class GetFoodItemByIdQuery : IRequest<FoodItem>
    {
        public int Id { get; set; }
    }
}
