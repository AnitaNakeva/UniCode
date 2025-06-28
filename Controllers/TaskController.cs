using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.DataModels;
using UniCodeProject.API.Services;

namespace UniCodeProject.API.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize(Roles = "Lecturer")]
    public class TaskController : ControllerBase
    {
        private readonly ILecturerService _lecturerService;
        private readonly ITaskService _taskService;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskController(
            ILecturerService lecturerService,
            ITaskService taskService,
            UserManager<ApplicationUser> userManager)
        {
            _lecturerService = lecturerService;
            _taskService = taskService;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
        {
            var userId = User.FindFirstValue("uid");
            Console.WriteLine($"[DEBUG] Got UID (from custom claim): {userId}");

            if (userId == null)
                return Unauthorized(new { message = "Invalid token." });

            var lecturer = await _lecturerService.GetByUserIdAsync(userId);
            if (lecturer == null)
                return BadRequest(new { message = $"No Lecturer profile found for user ID: {userId}" });

            taskModel.LecturerId = lecturer.UserId;
            taskModel.Lecturer = await _userManager.FindByIdAsync(userId);

            if (!taskModel.IsValid())
                return BadRequest(new { message = "Invalid task setup. Must have UnitTestCode or ExpectedOutput." });

            if (taskModel.StartTime >= taskModel.EndTime)
                return BadRequest(new { message = "Start time must be before end time." });

            if (taskModel.MaxScore <= 0)
                return BadRequest(new { message = "Max score must be greater than zero." });

            var createdTask = await _taskService.CreateTaskAsync(taskModel);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            if (task == null)
                return NotFound(new { message = "Task not found." });

            return Ok(task);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = User.FindFirstValue("uid");
            Console.WriteLine($"[DEBUG] Got UID for GetTasks: {userId}");

            if (userId == null)
                return Unauthorized(new { message = "Invalid token." });

            var tasks = await _taskService.GetTasksByLecturerAsync(userId);
            return Ok(tasks);
        }
    }
}
