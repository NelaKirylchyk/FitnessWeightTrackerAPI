using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Models.CustomValidation
{
    public class DateBeforeNowValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date <= DateTime.UtcNow)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Date cannot be in the future.");
        }
    }
}
