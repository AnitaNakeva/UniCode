using UniCodeProject.API.DTOs;

namespace UniCodeProject.API.Contracts;

public interface IStatisticsService
{
    Task<UniversityRankingDto> GetUniversityRankingAsync(string userId);
}
