using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.CustomExceptions
{
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
