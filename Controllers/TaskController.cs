using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly DockerExecutionService _dockerService;

        public TaskController(
            ILecturerService lecturerService,
                UserManager<ApplicationUser> userManager,
                    DockerExecutionService dockerService)
        {
            _lecturerService = lecturerService;
            _userManager = userManager;
            _dockerService = dockerService;
        }

        [HttpPost("execute")]
        public async Task<IActionResult> ExecuteCode([FromBody] CodeRequest request)
        {
            var output = await _dockerService.ExecuteCodeAsync(request.Code);
            return Ok(new { result = output });
        }

        public class CodeRequest
        {
            public string Code { get; set; }
        }

        // CREATE TASK (POST: api/tasks)
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel taskModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            if (taskModel.LecturerId != null && taskModel.LecturerId != userId)
            {
                return BadRequest(new { message = "You cannot assign a task to another lecturer." });
            }

            taskModel.LecturerId = userId;

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

            var createdTask = await _lecturerService.CreateTaskAsync(taskModel);

            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        // UPDATE TASK (PUT: api/tasks/{id})
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskModel taskModel)
        {
            if (id != taskModel.Id)
            {
                return BadRequest(new { message = "Task ID mismatch." });
            }

            var userId = _userManager.GetUserId(User);
            if (taskModel.LecturerId != userId)
            {
                return Forbid();
            }

            var existingTask = await _lecturerService.GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                return NotFound(new { message = "Task not found." });
            }

            if (!taskModel.IsValid())
            {
                return BadRequest(new { message = "Invalid task setup. Must have UnitTestCode or ExpectedOutput." });
            }

            taskModel.LastUpdatedAt = DateTime.UtcNow;
            var updatedTask = await _lecturerService.UpdateTaskAsync(taskModel);

            return Ok(updatedTask);
        }

        // DELETE TASK (DELETE: api/tasks/{id})
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _lecturerService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound(new { message = "Task not found." });
            }

            if (task.LecturerId != userId)
            {
                return Forbid();
            }

            await _lecturerService.DeleteTaskAsync(id);
            return NoContent();
        }

        // GET TASK BY ID (GET: api/tasks/{id})
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var task = await _lecturerService.GetTaskByIdAsync(id);

            if (task == null)
            {
                return NotFound(new { message = "Task not found." });
            }

            return Ok(task);
        }

        // GET TASKS BY LECTURER (GET: api/tasks)
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var userId = _userManager.GetUserId(User);
            var tasks = await _lecturerService.GetTasksByLecturerAsync(userId);

            return Ok(tasks);
        }
    }
}
