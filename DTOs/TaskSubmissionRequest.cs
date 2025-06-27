namespace UniCodeProject.API.DTOs
{
    public class TaskSubmissionRequest
    {
        public int TaskId { get; set; }
        public string Code { get; set; } = null!;
    }
}
