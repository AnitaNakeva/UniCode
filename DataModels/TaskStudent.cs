namespace UniCodeProject.API.DataModels
{
    public class TaskStudent
    {
        public int TaskId { get; set; }
        public TaskModel Task { get; set; } = null!;

        public int StudentProfileId { get; set; }
        public StudentProfile StudentProfile { get; set; } = null!;

        public int Score { get; set; }
    }
}
