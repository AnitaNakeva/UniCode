using System.ComponentModel.DataAnnotations;
using UniCodeProject.API.Validation;
using static UniCodeProject.API.Common.DataConstants.Register;

namespace UniCodeProject.API.DTOs
{
    public class RegisterModel
    {
        [Required]
        [MaxLength(FirstNameMaxLength)]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters long.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(LastNameMaxLength)]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters long.")]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(EmailMaxLength)]
        [ValidRole]
        public string Email { get; set; }

        [Required]
        [MaxLength(PasswordMaxLength)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = null!;

    }
}
