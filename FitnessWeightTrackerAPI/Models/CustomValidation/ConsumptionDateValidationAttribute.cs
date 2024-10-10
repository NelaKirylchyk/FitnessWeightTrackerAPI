using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Models.CustomValidation
{
    public class ConsumptionDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date <= DateTime.Now)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Consumption date cannot be in the future.");
        }
    }

}
