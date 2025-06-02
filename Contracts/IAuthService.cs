using UniCodeProject.API.DataModels;
using UniCodeProject.API.DTOs;

namespace UniCodeProject.API.Contracts
{
    public interface IAuthService
    {
        public Task<string> CheckRole(string email);
        public Task CreateProfile(ApplicationUser user, string roleName, string university);
        public Task<string> CheckUniversity(ApplicationUser user, string roleName);
        
    }
}
