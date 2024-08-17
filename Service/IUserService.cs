using api.Extensions;
using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Service
{
    public interface IUserService
    {
        Task<AppUser> GetCurrentUserAsync();
    }
    
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            var username = _httpContextAccessor.HttpContext?.User.GetUserName();
            return await _userManager.FindByNameAsync(username);
        }
    }

}
