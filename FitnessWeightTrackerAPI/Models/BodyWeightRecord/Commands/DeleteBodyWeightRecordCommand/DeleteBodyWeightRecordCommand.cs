using FitnessWeightTrackerAPI.Services.Interfaces;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Commands.DeleteBodyWeightRecordCommand
{
    public class DeleteBodyWeightRecordCommand : IRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
