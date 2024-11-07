using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class UserLoginDTO
    {
        [Required]
        public string UsernameOrEmail { get; init; }

        [Required]
        public string Password { get; init; }
    }
}
