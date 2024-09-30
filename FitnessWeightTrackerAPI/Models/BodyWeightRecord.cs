using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public DateTime Date { get; set; }
        [Required]
        public float Weight { get; set; }
    }
}
