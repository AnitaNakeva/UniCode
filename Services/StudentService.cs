using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _data;

        public StudentService(ApplicationDbContext data)
        {
            _data = data;
        }

        public async Task<int> CalculateLeaderboardPosition(StudentProfile studentProfile)
        {
            var allStudentsInUniversity = await _data.StudentProfiles
                .Where(sp => sp.UniversityName == studentProfile.UniversityName)
                .OrderByDescending(sp => sp.TotalPoints)
                .ToListAsync();

            for (int i = 0; i < allStudentsInUniversity.Count; i++)
            {
                if (allStudentsInUniversity[i].UserId == studentProfile.UserId)
                {
                    return i + 1;
                }
            }

            return -1;
        }
    }
}
