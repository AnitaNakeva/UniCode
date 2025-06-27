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
        private readonly DockerExecutionService _dockerService;

        public TaskController(
            ILecturerService lecturerService,
            ITaskService taskService,                  
            UserManager<ApplicationUser> userManager,
            DockerExecutionService dockerService)
        {
            _lecturerService = lecturerService;
            _taskService = taskService;
            _userManager = userManager;
            _dockerService = dockerService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized(new { message = "Invalid token." });

            var lecturer = await _lecturerService.GetByUserIdAsync(userId);
            if (lecturer == null)
                return BadRequest(new { message = "Lecturer profile not found." });

            taskModel.LecturerId = lecturer.Id.ToString();

            var user = await _userManager.FindByIdAsync(userId);
            taskModel.Lecturer = user;

            if (!taskModel.IsValid())
            {
                return BadRequest(new { message = "Invalid task setup. Must have UnitTestCode or ExpectedOutput." });
            }

            if (taskModel.StartTime >= taskModel.EndTime)
            {
                return BadRequest(new { message = "Start time must be before end time." });
            }

            if (taskModel.MaxScore <= 0)
            {
                return BadRequest(new { message = "Max score must be greater than zero." });
            }

            var createdTask = await _taskService.CreateTaskAsync(taskModel);

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskModel taskModel)
        {
            if (id != taskModel.Id)
                return BadRequest(new { message = "Task ID mismatch." });

            var userId = _userManager.GetUserId(User);
            if (taskModel.LecturerId != userId)
                return Forbid();

            var existingTask = await _taskService.GetTaskByIdAsync(id);
            if (existingTask == null)
                return NotFound(new { message = "Task not found." });

            if (!taskModel.IsValid())
                return BadRequest(new { message = "Invalid task setup. Must have UnitTestCode or ExpectedOutput." });

            taskModel.LastUpdatedAt = DateTime.UtcNow;
            var updatedTask = await _taskService.UpdateTaskAsync(taskModel);

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _taskService.GetTaskByIdAsync(id);

            if (task == null)
                return NotFound(new { message = "Task not found." });

            if (task.LecturerId != userId)
                return Forbid();

            await _taskService.DeleteTaskAsync(id);
            return NoContent();
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
            var userId = _userManager.GetUserId(User);
            var tasks = await _taskService.GetTasksByLecturerAsync(userId);

            return Ok(tasks);
        }
    }
}
