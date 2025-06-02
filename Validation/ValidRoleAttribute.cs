using System.ComponentModel.DataAnnotations;
using UniCodeProject.API.Data;

namespace UniCodeProject.API.Validation
{
    public class ValidRoleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult("Email is required.");
            }

            var dbContext = (ApplicationDbContext)validationContext.GetService(typeof(ApplicationDbContext));

            var lecturerDomains = dbContext.LecturerEmailDomains.Select(d => d.Domain).ToList();
            var studentDomains = dbContext.StudentEmailDomains.Select(d => d.Domain).ToList();

            string role = "no role";

            if (lecturerDomains.Any(domain => email.EndsWith(domain)))
            {
                role = "Lecturer";
            }
            else if (studentDomains.Any(domain => email.EndsWith(domain)))
            {
                role = "Student";
            }

            if (role == "no role")
            {
                return new ValidationResult("Invalid email domain. Registration is only allowed for students and teachers.");
            }

            validationContext.Items["UserRole"] = role;

            return ValidationResult.Success;
        }
    }
}
