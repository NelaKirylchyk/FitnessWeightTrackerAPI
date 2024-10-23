using System.ComponentModel.DataAnnotations;

namespace FitnessWeightTrackerAPI.Data.DTO
{
    public class UserDTO
    {
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
    
        public int Gender { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

    }
}
