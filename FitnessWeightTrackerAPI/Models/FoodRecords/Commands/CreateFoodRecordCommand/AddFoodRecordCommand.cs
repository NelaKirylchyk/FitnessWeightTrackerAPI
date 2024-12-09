using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.FoodRecords.Commands.CreateFoodRecordCommand
{
    public class AddFoodRecordCommand : IRequest<FoodRecord>
    {
        public int UserId { get; set; }

        public FoodRecordDTO Record { get; set; }
    }

}
