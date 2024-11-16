using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessWeightTrackerAPI.Models
{
    public class NutritionTarget
    {
        public int Id { get; set; }

        [ForeignKey("FitnessUser")]
        public int UserId { get; set; }

        public FitnessUser User { get; set; }

        [Range(0, 10000, ErrorMessage = "Daily calories must be between 0 and 10,000.")]
        public int DailyCalories { get; set; }

        [Range(0, 1000, ErrorMessage = "Daily carbohydrates must be between 0 and 1,000 grams.")]
        public int DailyCarbonohydrates { get; set; }

        [Range(0, 1000, ErrorMessage = "Daily protein must be between 0 and 1,000 grams.")]
        public int DailyProtein { get; set; }

        [Range(0, 1000, ErrorMessage = "Daily fat must be between 0 and 1,000 grams.")]
        public int DailyFat { get; set; }
    }
}
