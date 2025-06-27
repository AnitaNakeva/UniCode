using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Contracts
{
    public interface ITaskService
    {
        Task<TaskModel?> GetTaskByIdAsync(int id);
        Task<IEnumerable<TaskModel>> GetTasksByLecturerAsync(string lecturerId);
        Task SaveSubmissionAsync(TaskSubmission submission);
        Task<TaskModel?> UpdateTaskAsync(TaskModel updatedTask);
        Task DeleteTaskAsync(int id);
        Task<TaskModel> CreateTaskAsync(TaskModel task);
    }
}
