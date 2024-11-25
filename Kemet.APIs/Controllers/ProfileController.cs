using Kemet.APIs.DTOs;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{
    public class ProfileController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IinterestsRepository _interestRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProfileService _profileService;

        public ProfileController(
            UserManager<AppUser> userManager,
            IinterestsRepository interestRepository,
              IWebHostEnvironment webHostEnvironment,
              IProfileService profileService)
        {
            _userManager = userManager;
            _interestRepository = interestRepository;
            _webHostEnvironment = webHostEnvironment;
            _profileService = profileService;
        }

        // GET: api/Profile/GetCurrentUserData
        [HttpGet("GetCurrentUserData")]
        public async Task<IActionResult> GetCurrentUserData()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            if (user == null) return NotFound("User not found.");

            var userInterests = await _interestRepository.GetUserInterestsAsync(user.Id);

            var response = new GetCurrentUserDataResponseDto()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                SSN = user.SSN,
                Gender = user.Gender,
                Nationality = user.Nationality,
                ImageURL = user.ImageURL,
                InterestCategoryIds = userInterests
            };

            return Ok(response);
        }

        // PUT: api/Profile/UpdateUserData
        [HttpPut("UpdateUserData")]
        public async Task<IActionResult> UpdateUserData(UpdateUserDataDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            if (user == null) return NotFound("User not found.");

            // Update user fields only if they are provided
            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            if (dto.DateOfBirth != default(DateOnly))
                user.DateOfBirth = dto.DateOfBirth;

            if (!string.IsNullOrEmpty(dto.Gender))
                user.Gender = dto.Gender;

            if (!string.IsNullOrEmpty(dto.Nationality))
                user.Nationality = dto.Nationality;

            // Update user interests with InterestRepository if the list is not null
            if (dto.InterestCategoryIds != null && dto.InterestCategoryIds.Any())
                await _interestRepository.UpdateInterestsAsync(user.Id, dto.InterestCategoryIds);

            // Save changes to the user in the database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return StatusCode(500, "Failed to update user data.");

            return Ok("User data updated successfully.");
        }

        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            try
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

                var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
                if (user == null) return NotFound("User not found.");
                var result = await _profileService.UploadProfileImageAsync(user.Email, profileImage);

                if (!result.IsSuccess)
                {
                    return StatusCode(500, new { message = "Error uploading image." });
                }

                return Ok(new { message = "Image uploaded successfully.", filePath = result.ImageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
