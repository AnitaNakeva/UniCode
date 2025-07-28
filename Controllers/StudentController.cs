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

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = _userManager.GetUserId(User);
            var studentProfile = await _studentService.GetMyProfileWithTasksAndAchievementsAsync(userId);

            if (studentProfile == null)
                return NotFound(new { message = "Student profile not found." });

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
        
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] StudentProfile model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User);
            var updatedProfile = await _studentService.UpdateMyProfileAsync(userId, model);

            if (updatedProfile == null)
                return NotFound(new { message = "Student profile not found." });

            return Ok(updatedProfile);
        }


        [HttpGet("me/achievements")]
        public async Task<IActionResult> GetAchievements()
        {
            var userId = _userManager.GetUserId(User);
            var achievements = await _studentService.GetAchievementsAsync(userId);

            if (!achievements.Any())
                return NotFound(new { message = "No achievements found." });

            return Ok(achievements);
        }

    }
}
