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
        private readonly string _pPuploadsFolder;
        private readonly string _bGuploadsFolder;
        

        public ProfileService(UserManager<AppUser> userManager, string PPuploadsFolder , string BGuploadsFolder)
        {
            _userManager = userManager;
            _pPuploadsFolder = PPuploadsFolder;
            _bGuploadsFolder = BGuploadsFolder;
            
        }

        public async Task<(bool IsSuccess, string Message, string ImageUrl)> UploadProfileImageAsync(string userEmail, IFormFile profileImage, IFormFile backgroundImage)
        {
            if (profileImage == null && backgroundImage == null || profileImage.Length == 0 && backgroundImage.Length ==0)
            {
                return (false, "Please select an image to upload.", null);
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return (false, "User not found.", null);
            }

            // Ensure the directory exists
            if (!Directory.Exists(_pPuploadsFolder))
            {
                Directory.CreateDirectory(_pPuploadsFolder);
            }
            if (!Directory.Exists(_bGuploadsFolder))
            {
                Directory.CreateDirectory(_bGuploadsFolder);
            }

            if (profileImage != null)
            {
                string pPuniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
                string pPfilePath = Path.Combine(_pPuploadsFolder, pPuniqueFileName);
                using (var fileStream = new FileStream(pPfilePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }
                user.ImageURL = "/uploads/ProfileImages/" + pPuniqueFileName;

            }
            if(backgroundImage != null)
            {
                string bGuniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(profileImage.FileName);
                string bGfilePath = Path.Combine(_bGuploadsFolder, bGuniqueFileName);

                using (var fileStream = new FileStream(bGfilePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }

                user.BackgroundImageURL = "/uploads/BackGroundImages/" + bGuniqueFileName;
            }
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return (false, "An error occurred while updating the user profile.", null);
            }

            return (true, "Profile image uploaded successfully.", user.ImageURL);
        }
    }
}
