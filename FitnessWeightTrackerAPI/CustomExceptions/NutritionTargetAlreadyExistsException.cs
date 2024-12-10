namespace FitnessWeightTrackerAPI.CustomExceptions
{
    public class NutritionTargetAlreadyExistsException : Exception
    {
        public NutritionTargetAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
