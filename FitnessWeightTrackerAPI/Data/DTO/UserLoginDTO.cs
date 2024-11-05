using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class UserLoginDTO
    {
        [Required]
        public string UsernameOrEmail { get; init; }

        [Required]
        [StringLength(128)]
        public string Password { get; init; }
    }
}
