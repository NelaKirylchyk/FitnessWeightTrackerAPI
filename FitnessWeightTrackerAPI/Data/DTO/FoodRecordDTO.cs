namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class FoodRecordDTO
    {
        public int FoodItemId { get; set; }
        public FoodItemDTO FoodItem { get; set; }
        public float Quantity { get; set; }
        public DateTime ConsumptionDate { get; set; }
    }
}
