namespace FitnessWeightTracker.Client.Models
{
    // Minimal RFC 7807 compatible problem details for client-side deserialization
    public class ProblemDetails
    {
        public string? Type { get; set; }
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public string? Instance { get; set; }
        // Optional validation errors (like ValidationProblemDetails)
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}