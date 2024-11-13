using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessWeightTrackerAPI.Models.CustomValidation;

namespace FitnessWeightTrackerAPI.Models
{
    public class BodyWeightRecord
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DateRangeValidation]
        public DateTime Date { get; set; }

        [Required]
        [Range(0.01, 500.0, ErrorMessage = "Weight must be between 0.01 and 500.")]
        public float Weight { get; set; }
    }
}
