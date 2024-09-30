using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessWeightTrackerAPI.Models
{
    public class NutritionTarget
    {
        public long Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        public int DailyCalories { get; set; }
        [Required]
        public int DailyCarbonohydrates { get; set; }
        [Required]
        public int DailyProtein { get; set; }
        [Required]
        public int DailyFat { get; set; }
    }
}
