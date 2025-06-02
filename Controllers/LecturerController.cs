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

        // GET: api/lecturers/me
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _userManager.GetUserId(User);
            var lecturerProfile = await _context.LecturerProfiles
                .Include(lp => lp.Tasks)
                .FirstOrDefaultAsync(lp => lp.UserId == userId);

            if (lecturerProfile == null)
            {
                return NotFound(new { message = "Lecturer profile not found." });
            }

            return Ok(lecturerProfile);
        }

        // PUT: api/lecturers/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] LecturerProfile model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            var lecturerProfile = await _context.LecturerProfiles.FirstOrDefaultAsync(lp => lp.UserId == userId);

            if (lecturerProfile == null)
            {
                return NotFound(new { message = "Lecturer profile not found." });
            }

            lecturerProfile.FullName = model.FullName;
            lecturerProfile.ProfilePictureUrl = model.ProfilePictureUrl;
            lecturerProfile.Email = model.Email;

            _context.Update(lecturerProfile);
            await _context.SaveChangesAsync();

            return Ok(lecturerProfile);
        }

        // GET: api/lecturers/me/tasks
        [HttpGet("me/tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            var userId = _userManager.GetUserId(User);
            var tasks = await _context.LecturerProfiles
                .Where(lp => lp.UserId == userId)
                .SelectMany(lp => lp.Tasks)
                .ToListAsync();

            return Ok(tasks);
        }
        /*
         // GET: api/lecturers/{id}/tasks
        [HttpGet("{id}/tasks")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTasksByLecturerId(string id)
        {
            var tasks = await _context.LecturerProfiles
                .Where(lp => lp.UserId == id)
                .SelectMany(lp => lp.Tasks)
                .ToListAsync();

            if (!tasks.Any())
            {
                return NotFound(new { message = "No tasks found for this lecturer." });
            }

            return Ok(tasks);
        }*/
    }
}
