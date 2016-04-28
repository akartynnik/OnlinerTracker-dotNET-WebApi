using Microsoft.AspNet.Identity;
using OnlinerTracker.Data.SecurityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlinerTracker.Security
{
    public interface ISecurityRepository
    {
        Client FindClient(string clientId);

        Task<ApplicationUser> FindUserAsync(UserLoginInfo loginInfo);

        Task<IdentityResult> CreateUserAsync(ApplicationUser user);

        Task<IdentityResult> AddUserLoginAsync(string userId, UserLoginInfo login);

        IEnumerable<ApplicationUser> GetAllUsers();
    }
}
