using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.DataModels;
using UniCodeProject.API.DTOs;

namespace UniCodeProject.API.Controllers;

[ApiController]
[Route("api/stats")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _stats;
    private readonly UserManager<ApplicationUser> _userManager;

    public StatisticsController(IStatisticsService stats, UserManager<ApplicationUser> userManager)
    {
        _stats = stats;
        _userManager = userManager;
    }

    [Authorize(Roles = "Student,Lecturer")]
    [HttpGet("university-rank")]
    public async Task<ActionResult<UniversityRankingDto>> GetUniversityRank()
    {
        var userId = _userManager.GetUserId(User)!;
        var dto = await _stats.GetUniversityRankingAsync(userId);
        return Ok(dto);
    }
}
