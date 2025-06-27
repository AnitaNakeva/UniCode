using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;

        public TaskService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TaskModel?> GetTaskByIdAsync(int id)
        {
            return await _context.TaskModels
                .Include(t => t.Submissions)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskModel>> GetTasksByLecturerAsync(string lecturerId)
        {
            return await _context.TaskModels
                .Where(t => t.LecturerId == lecturerId)
                .ToListAsync();
        }

        public async Task<TaskModel> CreateTaskAsync(TaskModel task)
        {
            task.CreatedAt = DateTime.UtcNow;
            _context.TaskModels.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskModel?> UpdateTaskAsync(TaskModel updatedTask)
        {
            var existing = await _context.TaskModels.FindAsync(updatedTask.Id);
            if (existing == null) return null;

            existing.Name = updatedTask.Name;
            existing.Description = updatedTask.Description;
            existing.Language = updatedTask.Language;
            existing.TaskType = updatedTask.TaskType;
            existing.ExpectedOutput = updatedTask.ExpectedOutput;
            existing.UnitTestCode = updatedTask.UnitTestCode;
            existing.StartTime = updatedTask.StartTime;
            existing.EndTime = updatedTask.EndTime;
            existing.MaxScore = updatedTask.MaxScore;
            existing.LastUpdatedAt = updatedTask.LastUpdatedAt;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task DeleteTaskAsync(int id)
        {
            var task = await _context.TaskModels.FindAsync(id);
            if (task != null)
            {
                _context.TaskModels.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveSubmissionAsync(TaskSubmission submission)
        {
            _context.TaskSubmissions.Add(submission);
            await _context.SaveChangesAsync();
        }
    }
}
