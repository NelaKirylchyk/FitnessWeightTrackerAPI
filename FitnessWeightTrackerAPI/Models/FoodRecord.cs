using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FitnessWeightTrackerAPI.Models.CustomValidation;

namespace FitnessWeightTrackerAPI.Models
{
    public class FoodRecord
    {
        public int Id { get; set; }

        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }

        public FoodItem FoodItem { get; set; }

        [ForeignKey("FitnessUser")]
        public int UserId { get; set; }

        public FitnessUser User { get; set; }

        [Range(0.0, 10000.0, ErrorMessage = "Quantity must be a positive value.")]
        public float Quantity { get; set; }

        [DataType(DataType.Date)]
        [ConsumptionDateValidation]
        public DateTime ConsumptionDate { get; set; }
    }
}
