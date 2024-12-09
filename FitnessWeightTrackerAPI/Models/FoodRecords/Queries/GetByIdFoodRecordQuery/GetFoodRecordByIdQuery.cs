using MediatR;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Queries.GetByIdFoodRecordQuery
{
    public class GetFoodRecordByIdQuery : IRequest<FoodRecord>
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
