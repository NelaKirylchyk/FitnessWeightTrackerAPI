using FitnessWeightTrackerAPI.Services.Interfaces;
using MediatR;

namespace FitnessWeightTrackerAPI.Models.BodyWeightRecords.Queries.GetAllBodyWeightRecordsQuery
{
    public class GetBodyWeightRecordsQuery : IRequest<BodyWeightRecord[]>
    {
        public int UserId { get; set; }

        public bool AscendingOrder { get; set; }
    }

}
