using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class BodyWeightTargetDTO
    {
        public float TargetWeight { get; set; }
        public DateTime TargetDate { get; set; }
    }
}
