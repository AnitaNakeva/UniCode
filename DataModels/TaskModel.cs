namespace UniCodeProject.API.DataModels
{
    public enum TaskType
    {
        UnitTests,
        OutputBased
    }

    public class TaskModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public TaskType TaskType { get; set; }
        public string Language { get; set; } = null!;
        public string? UnitTestCode { get; set; }
        public string? ExpectedOutput { get; set; }

        public string? Password { get; set; }
        public string LecturerId { get; set; } = null!;
        public ApplicationUser Lecturer { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TaskSubmission> Submissions { get; set; } = new List<TaskSubmission>();

        public bool IsValid()
        {
            if (TaskType == TaskType.UnitTests && string.IsNullOrWhiteSpace(UnitTestCode))
                return false;

            if (TaskType == TaskType.OutputBased && string.IsNullOrWhiteSpace(ExpectedOutput))
                return false;

            return true;
        }
    }
}
