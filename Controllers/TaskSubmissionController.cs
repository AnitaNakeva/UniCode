using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;
using UniCodeProject.API.DTOs;
using UniCodeProject.API.Enums;
using UniCodeProject.API.Services;

namespace UniCodeProject.API.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/submit")]
    [Authorize(Roles = "Student")]
    public class TaskSubmissionController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly DockerExecutionService _dockerExecutionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TaskSubmissionController(
            ITaskService taskService,
            DockerExecutionService dockerExecutionService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _taskService = taskService;
            _dockerExecutionService = dockerExecutionService;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSolution(int taskId, [FromBody] CodeRequest request)
        {
            var studentId = _userManager.GetUserId(User);
            Console.WriteLine($"[DEBUG] Submitting as studentId: {studentId}");

            var task = await _taskService.GetTaskByIdAsync(taskId);

            if (task == null)
                return NotFound("Task not found.");

            if (task.TaskType == TaskType.UnitTests)
            {
                // Бъдеща поддръжка за UnitTests
                return BadRequest("Unit test evaluation is not implemented yet.");
            }

            if (task.TaskType != TaskType.OutputBased || string.IsNullOrWhiteSpace(task.ExpectedOutput))
                return BadRequest("Invalid or unsupported task configuration.");

            var submission = new TaskSubmission
            {
                TaskId = taskId,
                StudentId = studentId,
                Solution = request.Code,
                Status = SubmissionStatus.Pending,
                SubmissionTime = DateTime.UtcNow
            };

            await _taskService.SaveSubmissionAsync(submission);

            var startTime = DateTime.UtcNow;

            submission.Status = SubmissionStatus.Running;
            await _context.SaveChangesAsync();
            Console.WriteLine("[DEBUG] Set status to RUNNING and saved to DB");

            Console.WriteLine($"[DEBUG] Executing code for language: {task.Language}");
            var result = await _dockerExecutionService.ExecuteCodeAsync(submission.Solution, task.Language);

            Console.WriteLine($"[DEBUG] Docker returned: >{result}< expected: >{task.ExpectedOutput}<");

            var normalizedResult = result?.Trim()
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("Output:", "")
                .Replace("Error:", "")
                .Trim();

            var normalizedExpected = task.ExpectedOutput?.Trim()
                .Replace("\r", "")
                .Replace("\n", "");

            Console.WriteLine($"[DEBUG] Normalized: >{normalizedResult}< vs expected: >{normalizedExpected}<");

            submission.ExecutionResult = result;
            submission.Status = SubmissionStatus.Completed;
            submission.Score = normalizedResult == normalizedExpected ? task.MaxScore : 0;
            submission.Feedback = normalizedResult == normalizedExpected ? "Correct" : "Incorrect";
            submission.TimeTaken = DateTime.UtcNow - startTime;

            await _context.SaveChangesAsync();

            Console.WriteLine($"[DEBUG] Saved final submission: COMPLETED, score: {submission.Score}");



            return Ok(new
            {
                message = "Submission received and is being evaluated."
            });
        }
    }
}