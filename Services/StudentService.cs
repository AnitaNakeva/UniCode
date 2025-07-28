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
        
        public async Task<StudentProfile?> GetMyProfileWithTasksAndAchievementsAsync(string userId)
        {
            return await _data.StudentProfiles
                .Include(sp => sp.Achievements)
                .Include(sp => sp.TaskStudents)
                .ThenInclude(ts => ts.Task)
                .FirstOrDefaultAsync(sp => sp.UserId == userId);
        }
        
        public async Task<StudentProfile?> UpdateMyProfileAsync(string userId, StudentProfile updatedProfile)
        {
            var studentProfile = await _data.StudentProfiles.FirstOrDefaultAsync(sp => sp.UserId == userId);
            if (studentProfile == null) return null;

            studentProfile.FullName = updatedProfile.FullName;
            studentProfile.FacultyNumber = updatedProfile.FacultyNumber;
            studentProfile.ProfilePictureUrl = updatedProfile.ProfilePictureUrl;
            studentProfile.Email = updatedProfile.Email;

            _data.StudentProfiles.Update(studentProfile);
            await _data.SaveChangesAsync();

            return studentProfile;
        }

        public async Task<IEnumerable<Achievement>> GetAchievementsAsync(string userId)
        {
            return await _data.StudentProfiles
                .Where(sp => sp.UserId == userId)
                .SelectMany(sp => sp.Achievements)
                .ToListAsync();
        }


        
    }
}
