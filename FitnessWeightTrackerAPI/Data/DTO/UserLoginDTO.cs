namespace FitnessWeightTrackerAPI.Data.DTO
{
    using System.ComponentModel.DataAnnotations;

    public class UserLoginDTO
    {
        [Required]
        public string UsernameOrEmail { get; init; }

        [Required]
        public string Password { get; init; }
    }
}
