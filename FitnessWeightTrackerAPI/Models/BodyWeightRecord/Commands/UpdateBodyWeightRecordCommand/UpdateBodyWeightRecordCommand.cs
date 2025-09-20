using FitnessWeightTrackerAPI.Data.DTO;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.UpdateBodyWeightRecordCommand
{
    public class UpdateBodyWeightRecordCommand : IRequest<bool>
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public BodyWeightRecordDTO Record { get; set; }
    }

}
