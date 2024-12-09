using MediatR;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Queries.GetAllFoodRecordsQuery
{
    public class GetAllFoodRecordsQuery : IRequest<FoodRecord[]>
    {
        public int UserId { get; set; }

        public bool AscendingOrder { get; set; }
    }
}
