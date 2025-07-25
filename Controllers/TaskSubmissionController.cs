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
using Microsoft.Extensions.DependencyInjection;

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
        private readonly IServiceScopeFactory _scopeFactory;

        public TaskSubmissionController(
            ITaskService taskService,
            DockerExecutionService dockerExecutionService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IServiceScopeFactory scopeFactory)
        {
            _taskService = taskService;
            _dockerExecutionService = dockerExecutionService;
            _userManager = userManager;
            _context = context;
            _scopeFactory = scopeFactory;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSolution(int taskId, [FromBody] SubmissionRequest request)
        {
            var studentId = _userManager.GetUserId(User);

            var task = await _taskService.GetTaskByIdAsync(taskId);
            if (task == null)
                return NotFound("Task not found.");

            if (task.TaskType != TaskType.OutputBased || task.TestCases == null || !task.TestCases.Any())
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

            _ = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var scopedContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var scopedTaskService = scope.ServiceProvider.GetRequiredService<ITaskService>();

                var freshSubmission = await scopedContext.TaskSubmissions
                    .Include(s => s.Task)
                    .ThenInclude(t => t.TestCases)
                    .FirstOrDefaultAsync(s => s.Id == submission.Id);

                if (freshSubmission != null && freshSubmission.Task != null)
                {
                    await scopedTaskService.EvaluateSubmissionAsync(freshSubmission, freshSubmission.Task);
                }
            });
            
            return Ok(new
            {
                message = "Submission received and is being evaluated.",
                submissionId = submission.Id
            });
        }
        
        [HttpGet("/api/submissions/{submissionId}")]
        public async Task<IActionResult> GetSubmissionStatus(int submissionId)
        {
            var submission = await _context.TaskSubmissions
                .Where(s => s.Id == submissionId)
                .Select(s => new
                {
                    s.Id,
                    s.Status,
                    s.Score,
                    s.Feedback,
                    s.ExecutionResult
                })
                .FirstOrDefaultAsync();

            if (submission == null)
                return NotFound("Submission not found.");

            return Ok(submission);
        }
    }
}