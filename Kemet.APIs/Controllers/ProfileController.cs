using AutoMapper;
using Kemet.APIs.DTOs;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.ProfileDTOs;
using Kemet.APIs.Errors;
using Kemet.APIs.Extensions;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.ModelView;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public ProfileController(
            UserManager<AppUser> userManager,
            IinterestsRepository interestRepository,
              IWebHostEnvironment webHostEnvironment,
              IProfileService profileService,
              IConfiguration configuration,
              IMapper mapper)
        {
            _userManager = userManager;
            _interestRepository = interestRepository;
            _webHostEnvironment = webHostEnvironment;
            _profileService = profileService;
            _configuration = configuration;
            _mapper = mapper;
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
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                SSN = user.SSN,
                Gender = user.Gender,
                Nationality = user.Nationality,
                // Resolve Profile Image URL
                ProfileImageURL = !string.IsNullOrEmpty(user.ImageURL)
                        ? $"{_configuration["BaseUrl"]}/{user.ImageURL}"
                        : string.Empty,
                // Resolve Background Image URL
                BackgroundImageURL = !string.IsNullOrEmpty(user.BackgroundImageURL)
                        ? $"{_configuration["BaseUrl"]}/{user.BackgroundImageURL}"
                        : string.Empty,
                InterestCategoryIds = userInterests,
                Bio = user.Bio,
                WebsiteLink = user.WebsiteLink,
                CreationDate = user.CreationDate,
            };

            return Ok(response);
        }

        // PUT: api/Profile/UpdateUserData
        [HttpPut("UpdateUserData")]
       // [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUserData(UpdateUserDataDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            if (user == null) return NotFound("User not found.");

            #region Update User Fields
            // Update user fields only if they are provided
            if (!string.IsNullOrEmpty(dto.UserName))
                if(await _userManager.CheckUserNameExistsAsync(dto.UserName))
                {
                    user.UserName = dto.UserName;
                }
                else
                {
                    return BadRequest(new ApiResponse(400, "Username already exists"));
                }


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

            if (!string.IsNullOrEmpty(dto.Country))
                user.Country = dto.Country;

            if (!string.IsNullOrEmpty(dto.City))
                user.City = dto.City;

            if (!string.IsNullOrEmpty(dto.Bio))
                user.Bio = dto.Bio;

            if (!string.IsNullOrEmpty(dto.WebsiteLink))
                user.WebsiteLink = dto.WebsiteLink; 
            #endregion

            #region Customer interests Region
            // Update user interests with InterestRepository if the list is not null
            if (dto.InterestCategoryIds != null && dto.InterestCategoryIds.Any())
                await _interestRepository.UpdateInterestsAsync(user.Id, dto.InterestCategoryIds);

            #endregion

            // Save changes to the user in the database
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return StatusCode(500, "Failed to update user data.");

            return Ok("User data updated successfully.");
        }

        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile model)
        {
            if (model==null||model.Length==0)
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
                var result = await _profileService.UploadProfileImageAsync(user.Email, model, null);

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
        [HttpPost("upload-background-image")]
        public async Task<IActionResult> UploadBackgroundImage([FromForm] IFormFile model)
        {
            if ( model==null||model.Length==0)
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
                var result = await _profileService.UploadProfileImageAsync(user.Email,null,model);

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
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetAdventureMode()
        {

            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            if (user == null) return NotFound("User not found.");
            var adventureObject = await _profileService.GetAdventureModeSuggest(user);
            if (adventureObject.Place is null && adventureObject.Activity is null)
            {
                return BadRequest(new ApiResponse(400, "There was a proplem generating Adventure Mode Result !!"));
            }
            else
            {
                var result = new AdventureModeToReturn()
                {
                    placeDto = _mapper.Map<Place, DetailedPlaceDto>(adventureObject.Place),
                    activityDTOs = _mapper.Map<Activity, DetailedActivityDTOs>(adventureObject.Activity)
                };
                return Ok(result);
            }
               
        } 
    }
}
