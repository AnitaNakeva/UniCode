using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Controllers
{
    [Route("api/lecturers")]
    [ApiController]
    [Authorize(Roles = "Lecturer")]
    public class LecturerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILecturerService _lecturerService;

        public LecturerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ILecturerService lecturerService)
        {
            _context = context;
            _userManager = userManager;
            _lecturerService = lecturerService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _userManager.GetUserId(User);
            var profile = await _lecturerService.GetMyProfileAsync(userId);

            if (profile == null)
                return NotFound(new { message = "Lecturer profile not found." });

            return Ok(profile);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] LecturerProfile model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);
            var updatedProfile = await _lecturerService.UpdateMyProfileAsync(userId, model);

            if (updatedProfile == null)
                return NotFound(new { message = "Lecturer profile not found." });

            return Ok(updatedProfile);
        }

        [HttpGet("me/tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = _userManager.GetUserId(User);
            var tasks = await _lecturerService.GetMyTasksAsync(userId);
            return Ok(tasks);
        }

    }
}
