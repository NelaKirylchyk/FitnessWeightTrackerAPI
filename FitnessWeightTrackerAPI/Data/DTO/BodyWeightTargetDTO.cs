using FitnessWeightTrackerAPI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class BodyWeightTargetDTO
    {
        [Range(0.01, 500.0, ErrorMessage = "Target weight must be between 0.01 and 500.")]
        public float TargetWeight { get; set; }

        [DataType(DataType.Date)]
        [TargetDateValidation]
        public DateTime TargetDate { get; set; }
    }
}
