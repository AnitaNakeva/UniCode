using System.ComponentModel.DataAnnotations.Schema;

namespace UniCodeProject.API.DataModels
{
    public class StudentProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? ProfilePictureUrl { get; set; }

        public string FullName { get; set; } = null!;

        public string Status { get; set; } = "Student";

        public string? FacultyNumber { get; set; }

        public string Email { get; set; } = null!;

        public int TotalPoints { get; set; }

        public int PercentFinishedTasks { get; set; }

        public string UniversityName { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Achievement> Achievements { get; set; } = new List<Achievement>();

        public ICollection<TaskStudent> TaskStudents { get; set; } = new List<TaskStudent>();

        public virtual ApplicationUser User { get; set; } = null!;


        [NotMapped]
        public int PlaceInLeaderboard { get; set; }
    }
}
