namespace FitnessWeightTrackerAPI.Models.Extensions
{
    public static class GenderExtensions
    {
        public static GenderEnum ToGenderEnum(this int value)
        {
            if (Enum.IsDefined(typeof(GenderEnum), value))
            {
                return (GenderEnum)value;
            }

            return GenderEnum.NotSpecified; // Default to NotSpecified
        }
    }
}
