using Kemet.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kemet.APIs.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<bool> CheckUserNameExistsAsync(this UserManager<AppUser> userManager , string UserName)
        {
            
            var user = await userManager.Users.FirstOrDefaultAsync(U => U.UserName == UserName);
            if (user == null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
