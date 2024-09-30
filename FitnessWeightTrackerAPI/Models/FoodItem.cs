using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessWeightTrackerAPI.Models
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        public int Calories { get; set; }
        [Required]
        public int Carbohydrates { get; set; }
        [Required]
        public int Protein { get; set; }
        [Required]
        public int Fat {  get; set; }
        [Required]
        public int ServingSize { get; set; }
    }
}
