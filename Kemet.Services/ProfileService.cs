using Kemet.Core.Entities.Identity;
using Kemet.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly string _uploadsFolder;

        public ProfileService(UserManager<AppUser> userManager, string uploadsFolder)
        {
            _userManager = userManager;
            _uploadsFolder = uploadsFolder;
        }

        public async Task<(bool IsSuccess, string Message, string ImageUrl)> UploadProfileImageAsync(string userEmail, IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return (false, "Please select an image to upload.", null);
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return (false, "User not found.", null);
            }

            // Ensure the directory exists
            if (!Directory.Exists(_uploadsFolder))
            {
                Directory.CreateDirectory(_uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
            string filePath = Path.Combine(_uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(fileStream);
            }

            user.ImageURL = "/uploads/" + uniqueFileName;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return (false, "An error occurred while updating the user profile.", null);
            }

            return (true, "Profile image uploaded successfully.", user.ImageURL);
        }
    }
}
