using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Models
{
    public class FoodItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        public string Name { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Calories must be between 0 and 10,000.")]
        public int Calories { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Carbohydrates must be between 0 and 1,000 grams.")]
        public int Carbohydrates { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Protein must be between 0 and 1,000 grams.")]
        public int Protein { get; set; }

        [Required]
        [Range(0, 1000, ErrorMessage = "Fat must be between 0 and 1,000 grams.")]
        public int Fat { get; set; }

        [Required]
        [Range(0, 10000, ErrorMessage = "Serving size must be between 0 and 10,000 grams.")]
        public int ServingSize {  get; set; }
    }
}
