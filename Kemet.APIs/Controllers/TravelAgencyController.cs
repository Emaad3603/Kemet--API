using Kemet.APIs.DTOs.IdentityDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.ModelView;
using Kemet.Core.Services.Interfaces;
using Kemet.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{
    public class TravelAgencyController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITravelAgencyService _travelAgencyService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IProfileService _profileService;

        public TravelAgencyController(
            UserManager<AppUser> userManager ,
            ITravelAgencyService travelAgencyService ,
            IConfiguration configuration ,
            IWebHostEnvironment webHostEnvironment,
            IProfileService profileService 
            )
        {
            _userManager = userManager;
            _travelAgencyService = travelAgencyService;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _profileService = profileService;
        }
        [HttpGet]
        public async Task<TravelAgencyProfileDto> GetTravelAgnecy (string travelAgencyName)
        {
            var travelAgency = await _userManager.FindByNameAsync(travelAgencyName) as TravelAgency;
            var plans =  await _travelAgencyService.GetTravelAgencyPlans(travelAgency.Id , _configuration);
            var reviews = await _travelAgencyService.GetTravelAgencyReviews(travelAgency.Id , _configuration);
            double averageRating = reviews.Any()
                                          ? reviews.Average(r => r.Rating)
                                          : 0;
            averageRating = Math.Round(averageRating, 1);
            var result = new TravelAgencyProfileDto()
            { 
                TravelAgencyId = travelAgency.Id,
                UserName = travelAgency.UserName,
                Address = travelAgency.Address,
                Email = travelAgency.Email,
                FacebookURL = travelAgency.FacebookURL,
                InstagramURL = travelAgency.InstagramURL,
                BackgroundURL = travelAgency.BackgroundImageURL,
                ProfileURl = travelAgency.ImageURL,
                Bio = travelAgency.Bio,
                Description = travelAgency.Description,
                PhoneNumber = travelAgency.PhoneNumber,
                Plan = plans,
                reviews = reviews,
                AverageRating = averageRating
                
            };
            if (result == null)  NotFound(new ApiResponse(404, "No Travel Agency found."));
            return result;
        }

        [HttpGet("GetCustomers")]
        public async Task<IActionResult> GetCustomersAsync()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
            if (user == null) return NotFound("User not found.");

            DateTime now = DateTime.Now;

            // First day of last month
            DateTime firstDayLastMonth = new DateTime(now.Year, now.Month, 1).AddMonths(-1);

            var customers =  await _travelAgencyService.GetCustomersAsync(user.UserName);
            var NumOfCustomers = customers.Count();
            var NumOFNewCustomers = customers.Where(c=>c.CreatedAt >firstDayLastMonth ).Count();

            return Ok(new
            {
                Customers = customers,
                NumOfCustomers = NumOfCustomers,
                NumOfNewCustomers = NumOFNewCustomers
            });

        }

        [HttpGet("GetTravelAgencyReviewStats")]
        public async Task<IActionResult> GetTravelAgencyReviewsStats()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
            if (user == null) return NotFound("User not found.");

            var reviews =  await  _travelAgencyService.GetTravelAgencyReviews(user.Id, _configuration);
            var stats = await _travelAgencyService.CalculateSatisfactionRateAsync(user.Id);
            if(reviews.IsNullOrEmpty()) { reviews = new List<Review>(); }
            return Ok(new
            {
                reviews = reviews,
                ratingStats = stats.ratingCounts,
                SatisfactionRate = stats.satisfactionRate
            });
        }
        [HttpGet("GetTravelAgencyPlanStats")]
        public async Task<IActionResult> GetTravelAgencyPlanStats()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
            if (user == null) return NotFound("User not found.");
            var plans = await  _travelAgencyService.GetTravelAgencyPlans(user.Id, _configuration);
            var revenue = await _travelAgencyService.GetMonthlyRevenueAsync(user.UserName);
            var topPlans = await _travelAgencyService.GetTopBookedPlansAsync(user.UserName);

            return Ok(new
            {
                plans = plans,
                revenue = revenue,
                topPlans = topPlans
            });
        }

        [HttpGet("GetTravelAgnecyDashboard")]
        public async Task<IActionResult> GetTravelAgnecyDashboard()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var travelAgency = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
            if (travelAgency == null) return NotFound("User not found.");
          
            var reviews = await _travelAgencyService.GetTravelAgencyReviews(travelAgency.Id, _configuration);
            double averageRating = reviews.Any()
                                          ? reviews.Average(r => r.Rating)
                                          : 0;
            averageRating = Math.Round(averageRating, 1);
            var result = new TravelAgencyProfileDto()
            {
                TravelAgencyId = travelAgency.Id,
                UserName = travelAgency.UserName,
                Address = travelAgency.Address,
                Email = travelAgency.Email,
                FacebookURL = travelAgency.FacebookURL,
                InstagramURL = travelAgency.InstagramURL,
                BackgroundURL = $"{_configuration["BaseUrl"]}/{travelAgency.BackgroundImageURL}",
                ProfileURl = $"{_configuration["BaseUrl"]}/{travelAgency.ImageURL}",
                Bio = travelAgency.Bio,
                Description = travelAgency.Description,
                PhoneNumber = travelAgency.PhoneNumber,
                AverageRating = averageRating

            };
            if (result == null) NotFound(new ApiResponse(404, "No Travel Agency found."));
            return Ok(result);
        }

        [HttpPut("EditTravelAgencyProfile")]
        public async Task<IActionResult> EditTravelAgencyProfile([FromBody] EditTravelAgencyProfileDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var travelAgency = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
            if (travelAgency == null) return NotFound("User not found.");

            // Update fields
            travelAgency.Address = dto.Address ?? travelAgency.Address;
            travelAgency.FacebookURL = dto.FacebookURL ?? travelAgency.FacebookURL;
            travelAgency.InstagramURL = dto.InstagramURL ?? travelAgency.InstagramURL;
            travelAgency.BackgroundImageURL = travelAgency.BackgroundImageURL;
            travelAgency.ImageURL = travelAgency.ImageURL;
            travelAgency.Bio = dto.Bio ?? travelAgency.Bio;
            travelAgency.Description = dto.Description ?? travelAgency.Description;
            travelAgency.PhoneNumber = dto.PhoneNumber ?? travelAgency.PhoneNumber;

            var result = await _userManager.UpdateAsync(travelAgency);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse(400, "Failed to update profile."));
            }

            return Ok(new ApiResponse(200, "Profile updated successfully."));
        }
        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfileImage([FromForm] IFormFile model)
        {
            if (model == null || model.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            try
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

                var user = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
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
            if (model == null || model.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            try
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

                var user = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
                if (user == null) return NotFound("User not found.");
                var result = await _profileService.UploadProfileImageAsync(user.Email, null, model);

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

        [HttpPost("add-plan")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddPlan([FromForm] CreatePlanDto dto)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var travelAgency = await _userManager.FindByEmailAsync(userEmail) as TravelAgency;
            if (travelAgency == null) return NotFound("User not found.");
            dto.TravelAgencyId = travelAgency.Id;
            var result = await _travelAgencyService.AddPlanAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Plan);
        }

        [HttpPut("update-plan")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdatePlan([FromForm] UpdatePlanDto dto)
        {
            var result = await _travelAgencyService.UpdatePlanAsync(dto);

            if (!result.IsSuccess)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

    }
}
