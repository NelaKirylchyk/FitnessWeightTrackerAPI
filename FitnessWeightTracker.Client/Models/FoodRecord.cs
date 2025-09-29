using System;
using FitnessWeightTracker.Client.Models;

namespace FitnessWeightTracker.Client.Models
{
    public class FoodRecord
    {
        public int Id { get; set; }
        public int FoodItemId { get; set; }
        public FoodItem? FoodItem { get; set; }
        public float Quantity { get; set; }
        public DateTime ConsumptionDate { get; set; }
    }
}