using FitnessWeightTrackerAPI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class BodyWeightRecordDTO
    {
        [Range(0.0, 500.0, ErrorMessage = "Weight must be between 0 and 500.")]
        public float Weight { get; set; }

        [DataType(DataType.Date)]
        [DateRangeValidation]
        public DateTime Date { get; set; }
    }
}
