namespace FitnessWeightTrackerAPI.Data.DTO
{
    using System.ComponentModel.DataAnnotations;
    using FitnessWeightTrackerAPI.Models.CustomValidation;

    public class FoodRecordDTO
    {
        public int FoodItemId { get; set; }

        public FoodItemDTO FoodItem { get; set; }

        [Range(0.0, 10000.0, ErrorMessage = "Quantity must be a positive value.")]
        public float Quantity { get; set; }

        [DataType(DataType.Date)]
        [ConsumptionDateValidation]
        public DateTime ConsumptionDate { get; set; }
    }
}
