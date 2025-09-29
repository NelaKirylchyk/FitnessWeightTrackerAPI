namespace FitnessWeightTracker.Client.Models;

public class BodyWeightTarget
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public float TargetWeight { get; set; }
    public DateTime TargetDate { get; set; }
}