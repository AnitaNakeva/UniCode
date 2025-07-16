using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;
using UniCodeProject.API.Enums;

namespace UniCodeProject.API.Services
{
    public class TaskService : ITaskService
    {
        private readonly ApplicationDbContext _context;
        private readonly DockerExecutionService _dockerExecutionService;

        public TaskService(ApplicationDbContext context, DockerExecutionService dockerExecutionService)
        {
            _context = context;
            _dockerExecutionService = dockerExecutionService;
        }

        public async Task<TaskModel?> GetTaskByIdAsync(int id)
        {
            return await _context.TaskModels
                .Include(t => t.Submissions)
                .Include(t => t.TestCases)
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
            var existing = await _context.TaskModels
                .Include(t => t.TestCases)
                .FirstOrDefaultAsync(t => t.Id == updatedTask.Id);

            if (existing == null)
                return null;

            existing.Name = updatedTask.Name;
            existing.Description = updatedTask.Description;
            existing.Language = updatedTask.Language;
            existing.TaskType = updatedTask.TaskType;
            existing.UnitTestCode = updatedTask.UnitTestCode;
            existing.StartTime = updatedTask.StartTime;
            existing.EndTime = updatedTask.EndTime;
            existing.MaxScore = updatedTask.MaxScore;
            existing.LastUpdatedAt = DateTime.UtcNow;

            // Replace test cases
            _context.TaskTestCases.RemoveRange(existing.TestCases);
            if (updatedTask.TestCases != null)
            {
                existing.TestCases = updatedTask.TestCases;
            }

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
        
        public async Task<TaskSubmission> EvaluateSubmissionAsync(TaskSubmission submission, TaskModel task)
        {
            submission.Status = SubmissionStatus.Running;
            await _context.SaveChangesAsync();

            int passedTests = 0;
            List<string> testResults = new();

            foreach (var testCase in task.TestCases)
            {
                var output = await _dockerExecutionService.ExecuteCodeAsync(
                    submission.Solution,
                    task.Language,
                    testCase.InputData ?? "");

                bool passed = output.Trim() == (testCase.ExpectedOutput ?? "").Trim();
                if (passed) passedTests++;
                testResults.Add($"Input: {testCase.InputData} → Output: {output.Trim()} vs Expected: {testCase.ExpectedOutput} => {(passed ? "PASS" : "FAIL")}");
            }

            submission.ExecutionResult = string.Join("\n", testResults);
            submission.Status = SubmissionStatus.Completed;

            double ratio = (double)passedTests / task.TestCases.Count;
            submission.Score = (int)Math.Round(ratio * task.MaxScore);
            submission.Feedback = $"{passedTests}/{task.TestCases.Count} tests passed.";
            submission.TimeTaken = DateTime.UtcNow - submission.SubmissionTime;

            await _context.SaveChangesAsync();
            return submission;
        }
    }
}
