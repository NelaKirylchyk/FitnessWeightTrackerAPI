using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessWeightTrackerAPI.Models
{
    public class FoodRecord
    {
        public int Id { get; set; }

        [Required]
        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }
        public FoodItem FoodItem { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public float Quantity { get; set; }

        [Required]
        public DateTime ConsumptionDate { get; set; }
    }
}
