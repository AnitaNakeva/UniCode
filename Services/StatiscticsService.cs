using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DTOs;

namespace UniCodeProject.API.Services;

using Microsoft.EntityFrameworkCore;

public class StatisticsService : IStatisticsService
{
    private readonly ApplicationDbContext _db;
    public StatisticsService(ApplicationDbContext db) => _db = db;

    public async Task<UniversityRankingDto> GetUniversityRankingAsync(string userId)
    {
        var myUni = await _db.StudentProfiles
            .Where(sp => sp.UserId == userId)
            .Select(sp => sp.UniversityName)
            .SingleAsync();

        var list = await _db.StudentProfiles
            .Where(sp => sp.UniversityName == myUni)
            .Select(sp => new
            {
                sp.UserId,
                sp.FullName,
                Total = _db.TaskSubmissions
                    .Where(ts => ts.StudentId == sp.UserId)
                    .Sum(ts => (int?)ts.Score) ?? 0
            })
            .ToListAsync();

        var ordered = list
            .OrderByDescending(x => x.Total)
            .ThenBy(x => x.FullName)
            .ThenBy(x => x.UserId)
            .ToList();

        var index = ordered.FindIndex(x => x.UserId == userId);
        var myTotal = ordered[index].Total;

        return new UniversityRankingDto
        {
            UniversityName = myUni,
            TotalStudents = ordered.Count,
            Place = index + 1,
            TotalPoints = myTotal
        };
    }
}
