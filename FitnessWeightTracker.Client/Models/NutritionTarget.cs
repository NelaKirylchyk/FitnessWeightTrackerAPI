namespace FitnessWeightTracker.Client.Models;

public class NutritionTarget
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int DailyCalories { get; set; }
    public int DailyCarbonohydrates { get; set; }
    public int DailyProtein { get; set; }
    public int DailyFat { get; set; }
}