using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OnlinerTracker.Data.SecurityModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlinerTracker.Security
{
    public class SecurityRepository : IDisposable
    {
        #region Fields and Properties

        private SecurityContext _ctx;

        private UserManager<ApplicationUser> _userManager;

        #endregion

        #region Constructors

        public SecurityRepository()
        {
            _ctx = new SecurityContext();
            _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(_ctx));
        }

        #endregion
        
        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        public async Task<ApplicationUser> FindUserAsync(UserLoginInfo loginInfo)
        {
            var user = await _userManager.FindAsync(loginInfo);
            return user;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user)
        {
            var result = await _userManager.CreateAsync(user);
            return result;
        }

        public async Task<IdentityResult> AddUserLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);
            return result;
        }

        public virtual IEnumerable<ApplicationUser> GetAllUsers()
        {
            return _ctx.Users;
        }

        #region IDisposable methods

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }

        #endregion
    }
}