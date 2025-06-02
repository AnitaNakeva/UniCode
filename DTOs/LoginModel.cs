using System.ComponentModel.DataAnnotations;
using static UniCodeProject.API.Common.DataConstants.Login;

namespace UniCodeProject.API.DTOs
{
    public class LoginModel
    {
        [Required]
        [MaxLength(EmailMaxLength)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(PasswordMaxLength)]
        public string Password { get; set; } = null!;
    }
}
