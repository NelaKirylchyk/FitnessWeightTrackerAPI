using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTracker.Client.Models
{
    public class FoodRecordDTO
    {
        [Required]
        public int FoodItemId { get; set; }

        [Range(0.0, 10000.0, ErrorMessage = "Quantity must be a positive value.")]
        public float Quantity { get; set; }

        [DataType(DataType.Date)]
        public DateTime ConsumptionDate { get; set; }
    }
}