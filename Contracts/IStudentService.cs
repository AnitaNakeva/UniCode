using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Contracts
{
    public interface IStudentService
    {
        public Task<int> CalculateLeaderboardPosition(StudentProfile studentProfile);
    }
}
