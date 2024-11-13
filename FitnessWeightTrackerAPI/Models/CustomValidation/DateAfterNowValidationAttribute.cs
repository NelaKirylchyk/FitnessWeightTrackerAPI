using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Models.CustomValidation
{
    public class DateAfterNowValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date > DateTime.UtcNow)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Target date must be in the future.");
        }
    }
}
