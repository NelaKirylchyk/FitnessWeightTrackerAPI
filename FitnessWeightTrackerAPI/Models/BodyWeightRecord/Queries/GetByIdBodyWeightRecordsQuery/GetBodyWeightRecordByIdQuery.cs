using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Queries.GetByIdBodyWeightRecordsQuery
{
    public class GetBodyWeightRecordByIdQuery : IRequest<BodyWeightRecord>
    {
        public int Id { get; set; }

        public int UserId { get; set; }
    }
}
