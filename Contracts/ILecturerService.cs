using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Contracts
{
    public interface ILecturerService
    {
        Task<LecturerProfile?> GetByUserIdAsync(string userId);
        Task<TaskModel> CreateTaskAsync(TaskModel task);
        Task<TaskModel?> GetTaskByIdAsync(int taskId);
        Task<IEnumerable<TaskModel>> GetTasksByLecturerAsync(string lecturerId);
        Task<TaskModel?> UpdateTaskAsync(TaskModel updatedTask);
        Task DeleteTaskAsync(int taskId);
        Task<int> CalculateNumberOfTasks(LecturerProfile lecturerProfile);
    }
}
