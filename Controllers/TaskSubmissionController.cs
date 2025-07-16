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

            var evaluatedSubmission = await _taskService.EvaluateSubmissionAsync(submission, task);

            return Ok(new
            {
                message = "Submission evaluated.",
                passed = evaluatedSubmission.Feedback,
                score = evaluatedSubmission.Score
            });
        }
    }
}