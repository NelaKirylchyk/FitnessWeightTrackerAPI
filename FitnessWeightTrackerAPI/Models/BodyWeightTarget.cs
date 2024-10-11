using FitnessWeightTrackerAPI.Models.CustomValidation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessWeightTrackerAPI.Models
{
    public class BodyWeightTarget
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        [Range(0.01, 500.0, ErrorMessage = "Target weight must be between 0.01 and 500.")]
        public float TargetWeight { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [TargetDateValidation]
        public DateTime TargetDate { get; set; }
    }
}