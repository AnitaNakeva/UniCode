using Microsoft.EntityFrameworkCore;
using UniCodeProject.API.Contracts;
using UniCodeProject.API.Data;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _data;

        public AuthService(ApplicationDbContext data)
        {
            _data = data;
        }

        public async Task<string> CheckRole(string email)
        {
            string role;
            var teacherDomains = await _data.LecturerEmailDomains
                .Select(d => d.Domain).ToListAsync();

            var studentDomains = await _data.StudentEmailDomains
                .Select(d => d.Domain).ToListAsync();

            foreach (var domain in teacherDomains)
            {
                if (email.EndsWith(domain))
                {
                    role = "Lecturer";
                    return role;
                }
            }

            foreach (var domain in studentDomains)
            {
                if (email.EndsWith(domain))
                {
                    role = "Student";
                    return role;
                }
            }

            role = "no role";

            return role;
        }

        public async Task<string> CheckUniversity(ApplicationUser user, string roleName)
        {
            if (roleName == "Student")
            {

                var studentDomains = await _data.StudentEmailDomains
                    .Select(d => d.Domain).ToListAsync();

                foreach (var domain in studentDomains)
                {
                    if (user.Email.EndsWith(domain))
                    {
                        return await _data.StudentEmailDomains
                            .Where(d => d.Domain == domain)
                            .Select(u => u.UniversityName)
                            .FirstOrDefaultAsync();
                    }
                }
            }
            else if (roleName == "Lecturer")
            {
                var teachersDomains = await _data.LecturerEmailDomains
                    .Select(d => d.Domain).ToListAsync();

                foreach (var domain in teachersDomains)
                {
                    if (user.Email.EndsWith(domain))
                    {
                        return await _data.LecturerEmailDomains
                            .Where(d => d.Domain == domain)
                            .Select(u => u.UniversityName)
                            .FirstOrDefaultAsync();
                    }
                }
            }

            return "No university";
        }

        public async Task CreateProfile(ApplicationUser user, string roleName, string university)
        {
            if (roleName == "Student")
            {
                int placeInLeaderboard = await _data.StudentProfiles
                    .Where(s => s.UniversityName == university)
                    .CountAsync() + 1;

                var profile = new StudentProfile
                {
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    UserId = user.Id,
                    TotalPoints = 0,
                    User = user,
                    PlaceInLeaderboard = placeInLeaderboard,
                    UniversityName = university,
                    CreatedAt = DateTime.UtcNow,
                    PercentFinishedTasks = 0,

                };

                _data.StudentProfiles.Add(profile);
                await _data.SaveChangesAsync();
            }
            else
            {
                var profile = new LecturerProfile
                {
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    UserId = user.Id,
                    User = user,
                    UniversityName = university,
                    CreatedAt = DateTime.UtcNow
                };

                _data.LecturerProfiles.Add(profile);
                await _data.SaveChangesAsync();
            }
        }
    }
}
