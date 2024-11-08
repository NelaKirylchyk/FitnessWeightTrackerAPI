namespace FitnessWeightTrackerAPI.Data.DTO
{
    using System.ComponentModel.DataAnnotations;

    public class RegistrationUserDTO
    {
        [Required]
        [StringLength(32, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 1)]
        public string Surname { get; set; }

        public int Gender { get; set; }

        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 1)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*" + "@" + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$", ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required]
        [StringLength(128, MinimumLength = 8)]
        public string Password { get; set; }
    }
}
