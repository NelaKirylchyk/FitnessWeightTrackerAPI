using MediatR;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Commands.DeleteFoodRecordCommand
{
    public class DeleteFoodRecordCommand : IRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
