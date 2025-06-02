namespace UniCodeProject.API.DataModels
{
    public class LecturerProfile
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? ProfilePictureUrl { get; set; }

        public string FullName { get; set; } = null!;

        public string Status { get; set; } = "Lecturer";

        public string Email { get; set; } = null!;

        public string UniversityName { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<TaskModel> Tasks { get; set; } = new List<TaskModel>();

        public virtual ApplicationUser User { get; set; } = null!;
    }
}
