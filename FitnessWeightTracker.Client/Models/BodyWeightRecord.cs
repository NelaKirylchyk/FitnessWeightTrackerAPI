namespace FitnessWeightTracker.Client.Models;

public class BodyWeightRecord
{
    public int Id { get; set; }
    public int UserId { get; set; } // Ignored by the API on POST/PUT; populated on GET
    public DateTime Date { get; set; }
    public float Weight { get; set; }
}