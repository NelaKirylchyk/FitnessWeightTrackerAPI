using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class BodyWeightRecordDTO
    {
        public float Weight { get; set; }

        public DateTime Date { get; set; }
    }
}
