namespace FitnessWeightTracker.Client.Models;

public class FoodItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Calories { get; set; }
    public int Carbohydrates { get; set; }
    public int Protein { get; set; }
    public int Fat { get; set; }
    public int ServingSize { get; set; }
}