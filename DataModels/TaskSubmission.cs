namespace UniCodeProject.API.DataModels
{
    public class TaskSubmission
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public TaskModel Task { get; set; } = null!;

        public string StudentId { get; set; } = null!;
        public ApplicationUser Student { get; set; } = null!;

        public int? Score { get; set; }
        public DateTime SubmissionTime { get; set; } = DateTime.UtcNow;
        public TimeSpan TimeTaken { get; set; }

        public string Solution { get; set; } = null!;
        public string? Feedback { get; set; }

        public string? ExecutionResult { get; set; }
    }
}
