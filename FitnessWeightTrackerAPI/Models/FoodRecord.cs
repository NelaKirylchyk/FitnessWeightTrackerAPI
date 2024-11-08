namespace FitnessWeightTrackerAPI.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using FitnessWeightTrackerAPI.Models.CustomValidation;

    public class FoodRecord
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }

        public FoodItem FoodItem { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        [Required]
        [Range(0.0, 10000.0, ErrorMessage = "Quantity must be a positive value.")]
        public float Quantity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [ConsumptionDateValidation]
        public DateTime ConsumptionDate { get; set; }
    }
}
