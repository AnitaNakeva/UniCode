using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Services
{
    public class LecturerService : ILecturerService
    {
        private readonly ApplicationDbContext _context;

        public LecturerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CalculateNumberOfTasks(LecturerProfile lecturerProfile)
        {
            var numberOfTasks = await _context.TaskModels
                .Where(t => t.LecturerId == lecturerProfile.UserId)
                .CountAsync();

            return numberOfTasks;
        }

        public async Task<TaskModel> CreateTaskAsync(TaskModel task)
        {
            _context.TaskModels.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<TaskModel> UpdateTaskAsync(TaskModel task)
        {
            _context.TaskModels.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task DeleteTaskAsync(int taskId)
        {
            var task = await _context.TaskModels.FindAsync(taskId);
            if (task != null)
            {
                _context.TaskModels.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<TaskModel> GetTaskByIdAsync(int taskId)
        {
            return await _context.TaskModels.FindAsync(taskId);
        }

        public async Task<IEnumerable<TaskModel>> GetTasksByLecturerAsync(string lecturerId)
        {
            return await _context.TaskModels.Where(t => t.LecturerId == lecturerId).ToListAsync();
        }

        public async Task<LecturerProfile?> GetByUserIdAsync(string userId)
        {
            var all = await _context.LecturerProfiles.ToListAsync();
            Console.WriteLine("Searching for UserId: " + userId);
            foreach (var l in all)
            {
                Console.WriteLine($"Lecturer DB: {l.UserId}");
            }

            return all.FirstOrDefault(p => p.UserId == userId);
        }

    }
}
