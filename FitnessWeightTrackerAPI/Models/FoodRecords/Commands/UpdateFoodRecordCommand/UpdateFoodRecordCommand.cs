using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Commands.UpdateFoodRecordCommand
{
    public class UpdateFoodRecordCommand : IRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public FoodRecordDTO Record { get; set; }
    }
}
