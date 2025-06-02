using System.ComponentModel.DataAnnotations;
using static UniCodeProject.API.Common.DataConstants.Domain;

namespace UniCodeProject.API.DataModels
{
    public class LecturerEmailDomain
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(DomainMaxLength)]
        public string Domain { get; set; } = null!;

        public string UniversityName { get; set; } = null!;
    }
}
