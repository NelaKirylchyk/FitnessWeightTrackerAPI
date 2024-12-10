namespace FitnessWeightTrackerAPI.CustomExceptions
{
    public class TargetAlreadyExistsException : Exception
    {
        public TargetAlreadyExistsException(string message) : base(message)
        {
        }
    }
}
