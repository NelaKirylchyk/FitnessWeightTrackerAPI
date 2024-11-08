namespace FitnessWeightTrackerAPI.CustomExceptions
{
    using System.ComponentModel.DataAnnotations;

    public class CustomValidationException : Exception
    {
        public CustomValidationException(IEnumerable<ValidationResult> validationResults)
            : base("Validation failed")
        {
            ValidationResults = validationResults;
        }

        public IEnumerable<ValidationResult> ValidationResults { get; }
    }
}
