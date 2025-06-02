using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Controllers
{
    [Route("api/students")]
    [ApiController]
    [Authorize(Roles = "Student")]
    public class StudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IStudentService studentService)
        {
            _context = context;
            _userManager = userManager;
            _studentService = studentService;
        }

        // GET: api/students/me
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _userManager.GetUserId(User);
            var studentProfile = await _context.StudentProfiles
                .Include(sp => sp.Achievements)
                .Include(sp => sp.TaskStudents)
                .ThenInclude(ts => ts.Task)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);

            if (studentProfile == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }

            studentProfile.PlaceInLeaderboard = await _studentService.CalculateLeaderboardPosition(studentProfile);

            var profileResponse = new
            {
                studentProfile.FullName,
                studentProfile.Email,
                studentProfile.FacultyNumber,
                studentProfile.TotalPoints,
                studentProfile.PlaceInLeaderboard,
                Achievements = studentProfile.Achievements,
                Tasks = studentProfile.TaskStudents.Select(ts => new
                {
                    ts.Task.Name,
                    ts.Score
                }).ToList()
            };

            return Ok(profileResponse);
        }

        // PUT: api/students/me
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] StudentProfile model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = _userManager.GetUserId(User);
            var studentProfile = await _context.StudentProfiles.FirstOrDefaultAsync(sp => sp.UserId == userId);

            if (studentProfile == null)
            {
                return NotFound(new { message = "Student profile not found." });
            }

            studentProfile.FullName = model.FullName;
            studentProfile.FacultyNumber = model.FacultyNumber;
            studentProfile.ProfilePictureUrl = model.ProfilePictureUrl;
            studentProfile.Email = model.Email;

            _context.StudentProfiles.Update(studentProfile);
            await _context.SaveChangesAsync();

            return Ok(studentProfile);
        }

        // GET: api/students/me/achievements
        [HttpGet("me/achievements")]
        public async Task<IActionResult> GetAchievements()
        {
            var userId = _userManager.GetUserId(User);
            var achievements = await _context.StudentProfiles
                .Where(sp => sp.UserId == userId)
                .SelectMany(sp => sp.Achievements)
                .ToListAsync();

            if (!achievements.Any())
            {
                return NotFound(new { message = "No achievements found." });
            }

            return Ok(achievements);
        }
    }
}
