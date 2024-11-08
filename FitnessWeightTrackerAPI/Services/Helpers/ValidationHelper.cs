namespace FitnessWeightTrackerAPI.Services.Helpers
{
    using System.ComponentModel.DataAnnotations;

    public static class ValidationHelper
    {
        public static bool TryValidateObject(object obj, out List<ValidationResult> results)
        {
            var context = new ValidationContext(obj);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, context, results, true);
        }
    }
}
