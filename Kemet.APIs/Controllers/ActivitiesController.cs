using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.ReviewsDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.PlaceSpecs;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{
   
    public class ActivitiesController : BaseApiController
    {
        private readonly IGenericRepository<Activity> _activityRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public ActivitiesController(
            IGenericRepository<Activity>ActivityRepo
            ,IMapper mapper
            ,AppDbContext context
            , UserManager<AppUser> userManager
            ,IOptions<Appsettings>appsettings
            ,IConfiguration configuration)
        {
            _activityRepo = ActivityRepo;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet] // /api/Activities  Get
        public async Task<ActionResult<IEnumerable<ActivityDTOs>>> GetActivity()
        {
            try
            {
                // Check if the user is signed in
                var userEmail = User.FindFirstValue(ClaimTypes.Email); // Assuming email is used for authentication
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Unauthorized(new ApiResponse(401, "User is not signed in."));
                }

                // Fetch the user's location from the database
                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null || user.Location == null)
                {
                    return NotFound(new ApiResponse(404, "User location not found."));
                }

                var userLocation = user.Location; // Assuming user.Location is a Point

                // Initialize radius (10 km)
                double radius = 10000; // 10 km in meters
                List<Activity> nearbyActivities = new List<Activity>();

                // Fetch activities within the radius, increasing the radius until at least 5 activities are found
                while (nearbyActivities.Count < 10)
                {
                    // Fetch activities within the current radius
                    var activitiesWithinRadius = await _context.Activities
                        .Where(a => a.Location.Coordinates.Distance(userLocation) <= radius)
                        .ToListAsync();

                    // Add activities to the nearbyActivities list
                    nearbyActivities.AddRange(activitiesWithinRadius);

                    // If no activities are found, break the loop to avoid infinite looping
                    if (!activitiesWithinRadius.Any())
                    {
                        break;
                    }

                    // Increase the radius by 10 km
                    radius += 10000;
                }

                // If no activities are found after increasing the radius, return a 404 response
                if (!nearbyActivities.Any())
                {
                    return NotFound(new ApiResponse(404, "No activities found within the maximum radius."));
                }

                // Take at least 5 activities (or all available if less than 5)
                var resultActivities = nearbyActivities.Take(10).ToList();

                // Map activities to DTOs
                var activitiesDto = _mapper.Map<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>(resultActivities);

                foreach (var Activity in activitiesDto)
                {
                    var images = await _context.ActivityImages.Where(p => p.ActivityId ==Activity.ActivityId).Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToListAsync();
                    Activity.imageURLs = images;
                }

                // Filter activities with images
                var result = activitiesDto.Where(a => a.imageURLs.Any()).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
       

        [HttpGet("GetActivityByID")]
        public async Task<ActionResult<ActivityDTOs>> GetActivityById(int ActivityID)
        {

            try
            {
                var spec = new ActivityWithPlacesSpecifications(ActivityID);
                var activity =await _activityRepo.GetWithSpecAsync(spec);
                if (activity == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
             
                var Activities = _mapper.Map<Activity, DetailedActivityDTOs>(activity);
                if (Activities == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
                var fetchedReviews =  await   _context.Reviews.Where(r=>r.ActivityId == ActivityID).ToListAsync();
                foreach(var fetchedReview in fetchedReviews)
                {
                    fetchedReview.ImageUrl = $"{_configuration["BaseUrl"]}{fetchedReview.ImageUrl}";
                }
                Activities.Reviews =fetchedReviews;
                var Result = Activities;
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

    }
}                       
                        