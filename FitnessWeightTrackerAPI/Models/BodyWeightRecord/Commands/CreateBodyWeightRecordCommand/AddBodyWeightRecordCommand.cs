using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.CreateBodyWeightRecordCommand
{
    public class AddBodyWeightRecordCommand : IRequest<BodyWeightRecord>
    {
        public int UserId { get; set; }

        public BodyWeightRecordDTO Record { get; set; }
    }
}
