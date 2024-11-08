namespace FitnessWeightTrackerAPI.Models
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        public int Gender { get; set; }

        public DateTime BirthDate { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }
}
