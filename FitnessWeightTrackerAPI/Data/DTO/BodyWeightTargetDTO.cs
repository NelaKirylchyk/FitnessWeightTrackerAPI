using FitnessWeightTrackerAPI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class BodyWeightTargetDTO
    {
        [Range(0.0, 500.0, ErrorMessage = "Target weight must be between 0 and 500.")]
        public float TargetWeight { get; set; }

        [DataType(DataType.Date)]
        [DateRangeValidation]
        public DateTime TargetDate { get; set; }
    }
}
