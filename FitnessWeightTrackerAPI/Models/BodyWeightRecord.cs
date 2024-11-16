using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessWeightTrackerAPI.Models.CustomValidation;

namespace FitnessWeightTrackerAPI.Models
{
    public class BodyWeightRecord
    {
        public int Id { get; set; }

        [ForeignKey("FitnessUser")]
        public int UserId { get; set; }

        public FitnessUser User { get; set; }

        [DataType(DataType.Date)]
        [DateBeforeNowValidation]
        public DateTime Date { get; set; }

        [Range(0.01, 500.0, ErrorMessage = "Weight must be between 0.01 and 500.")]
        public float Weight { get; set; }
    }
}
