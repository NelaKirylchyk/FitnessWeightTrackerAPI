﻿using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Models.CustomValidation
{
    public class TargetDateValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date && date > DateTime.Now)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Target date must be in the future.");
        }

    }
}
