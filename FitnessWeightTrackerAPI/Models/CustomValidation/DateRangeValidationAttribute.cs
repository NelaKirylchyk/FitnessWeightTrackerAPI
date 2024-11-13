using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Models.CustomValidation
{
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date <= DateTime.Now)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("Date cannot be in the future.");
        }
    }
}
