using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Contracts
{
    public interface ILecturerService
    {
        Task<int> CalculateNumberOfTasks(LecturerProfile lecturerProfile);
        Task<TaskModel> CreateTaskAsync(TaskModel task);
        Task<TaskModel> UpdateTaskAsync(TaskModel task);
        Task DeleteTaskAsync(int taskId);
        Task<TaskModel> GetTaskByIdAsync(int taskId);
        Task<IEnumerable<TaskModel>> GetTasksByLecturerAsync(string lecturerId);
    }
}
