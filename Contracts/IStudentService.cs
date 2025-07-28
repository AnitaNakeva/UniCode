using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Contracts
{
    public interface IStudentService
    {
        Task<StudentProfile?> GetMyProfileWithTasksAndAchievementsAsync(string userId);
        Task<StudentProfile?> UpdateMyProfileAsync(string userId, StudentProfile updatedProfile);
        Task<IEnumerable<Achievement>> GetAchievementsAsync(string userId);
        Task<int> CalculateLeaderboardPosition(StudentProfile studentProfile);

    }
}
