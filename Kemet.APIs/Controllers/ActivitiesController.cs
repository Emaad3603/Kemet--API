﻿using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.ReviewsDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
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
using System.Text.Json;

namespace Kemet.APIs.Controllers
{
   
    public class ActivitiesController : BaseApiController
    {
        
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHomeServices _homeServices;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheRepository _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public ActivitiesController(         
            IMapper mapper
            ,AppDbContext context
            ,UserManager<AppUser> userManager
            ,IHomeServices homeServices
            ,IConfiguration configuration 
            ,IUnitOfWork unitOfWork
            ,ICacheRepository cache
            )
        {
            
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _homeServices = homeServices;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<ActivityDTOs>> GetActivites()
        {
            try
            {
                string cacheKey = "Activities_list";
                var cached = await _cache.GetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cached))
                    return Ok(JsonSerializer.Deserialize<List<ActivityDTOs>>(cached)!);
                var resultActivities = await _homeServices.GetActivities();
                var result =await  MapActivitiesWithImages(resultActivities.Take(10).ToList());
                await _cache.SetAsync(cacheKey, result, _cacheDuration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("ActivitiesInCairo")]
        public async Task<ActionResult<ActivityDTOs>> GetActivitesInCairo()
        {
            try
            {
                string cacheKey = "Cairo_Activities_list";
                var cached = await _cache.GetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cached))
                    return Ok(JsonSerializer.Deserialize<List<ActivityDTOs>>(cached)!);
                var resultActivities = await _homeServices.GetActivitesInCairo();
                var result = await MapActivitiesWithImages(resultActivities);
                if(result == null) { return NotFound(new ApiResponse(404, "No activities found within the maximum radius.")); }
                await _cache.SetAsync(cacheKey, result, _cacheDuration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("ActivitiesHiddenGems")]
        public async Task<ActionResult<ActivityDTOs>> GetActivitesHiddenGems()
        {
            try
            {
                string cacheKey = "Hidden_Activities_list";
                var cached = await _cache.GetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cached))
                    return Ok(JsonSerializer.Deserialize<List<ActivityDTOs>>(cached)!);
                var resultActivities = await _homeServices.GetActivityHiddenGems();
                var result = await MapActivitiesWithImages(resultActivities);
                if (result == null) { return NotFound(new ApiResponse(404, "No activities found within the maximum radius.")); }
                await _cache.SetAsync(cacheKey, result, _cacheDuration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("ActivitiesTopRated")]
        public async Task<ActionResult<ActivityDTOs>> GetActivitesTopRated()
        {
            try
            {
                var resultActivities = await _homeServices.GetTopRatedActivities();
                var result = await MapActivitiesWithImages(resultActivities);
                if (result == null) { return NotFound(new ApiResponse(404, "No activities found within the maximum radius.")); }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("NearbyActivities")] // /api/Activities  Get
        public async Task<ActionResult<IEnumerable<ActivityDTOs>>> GetNearbyActivity()
        {
            try
            {
                // Check if the user is signed in
                string userEmail =  User.FindFirstValue(ClaimTypes.Email);

                // Fetch the user's location from the database
                var user = new AppUser();
                if (userEmail != null)
                {
                    user = await _userManager.FindByEmailAsync(userEmail);
                }
                var resultActivities = new List<Activity>();
                if (user == null || user.Location == null)
                {
                   resultActivities =  await _homeServices.GetActivities();
                   
                }
                else
                { 
                    var nearbyActivities = await _homeServices.GetNearbyActivities(user);
                    // If no activities are found after increasing the radius, return a 404 response
                    if (!nearbyActivities.Any())
                    {
                        return NotFound(new ApiResponse(404, "No activities found within the maximum radius."));
                    }
                    // Take at least 5 activities (or all available if less than 5)
                    resultActivities = nearbyActivities.Take(25).ToList();
                }

                var result = await MapActivitiesWithImages(resultActivities);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("GetActivityByID")]
        public async Task<ActionResult<DetailedActivityDTOs>> GetActivityById(int activityID)
        {
            try
            {
                // Fetch activity using specification pattern
                var spec = new ActivityWithPlacesSpecifications(activityID);
                var activity = await _unitOfWork.Repository<Activity>().GetWithSpecAsync(spec);

                if (activity == null)
                {
                    return NotFound(new ApiResponse(404, "No Activity found."));
                }

                // Map activity to DTO
                var activityDto = _mapper.Map<Activity, DetailedActivityDTOs>(activity);

                // Fetch and format reviews
                activityDto.Reviews = await GetFormattedReviews(activityID);
                activityDto.CategoryName = await _context.Categories.Where(c=>c.Id == activity.CategoryId).Select(c=>c.CategoryName).FirstOrDefaultAsync();
                return Ok(activityDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("B7B7")]
        public async Task<IActionResult> B7B7()
        {
            try
            {
                var Activities = await _context.Activities.ToListAsync();
                var B7B7Activities = new List<B7B7ActivityDto>();
                foreach (var Activity in Activities)
                {
                    var images = await _context.ActivityImages.Where(p => p.ActivityId == Activity.Id).Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").FirstOrDefaultAsync();
                    Activity.PictureUrl = images;
                }
                foreach (var Activity in Activities)
                {
                    var test = new B7B7ActivityDto
                    {
                        ActivityId = Activity.Id,
                        CategoryName = await _context.Categories
                            .Where(c => c.Id == Activity.CategoryId)
                            .Select(c => c.CategoryName)
                            .FirstOrDefaultAsync(),
                        Name = Activity.Name,
                        CulturalTips = Activity.CulturalTips,
                        Address = await _context.Locations
                            .Where(l => l.Id == Activity.LocationId)
                            .Select(l => l.Address)
                            .FirstOrDefaultAsync(),
                        Duration = Activity.Duration,
                        CloseTime = Activity.CloseTime,
                        OpenTime = Activity.OpenTime,
                        Description = Activity.Description,
                        imageURL = Activity.PictureUrl,
                        LocationLink = await _context.Locations
                            .Where(l => l.Id == Activity.LocationId)
                            .Select(l => l.LocationLink)
                            .FirstOrDefaultAsync(),
                        TouristAdult = await _context.Prices
                            .Where(p => p.Id == Activity.priceId)
                            .Select(p => p.TouristAdult)
                            .FirstOrDefaultAsync(),
                        EgyptianAdult = await _context.Prices
                            .Where(p => p.Id == Activity.priceId)
                            .Select(p => p.EgyptianAdult)
                            .FirstOrDefaultAsync()
                    };

                    B7B7Activities.Add(test);
                }
                return Ok(B7B7Activities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("badrequest")]
        public async Task<ActionResult> GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("unauthorized")]
        public async Task<ActionResult> GetUnauthorizedError()
        {
            return Unauthorized(new ApiResponse(401));
        }
        [HttpGet("ActivitesForYou")]
        public async Task<ActionResult> GetActivitiesWithInterests()
        {
            try
            {
                // Check if the user is signed in
                string userEmail = User.FindFirstValue(ClaimTypes.Email);

                // Fetch the user's location from the database
                var user = new AppUser();
                if (userEmail != null)
                {
                    user = await _userManager.FindByEmailAsync(userEmail);
                }

                var activities = await _homeServices.GetActivitiesByCustomerInterests(user.Id);
                if (activities == null || activities.Count == 0)
                {
                    return NotFound();
                }
                var resActivities = await MapActivitiesWithImages(activities);
                return Ok(resActivities);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        private async Task<List<ActivityDTOs>> MapActivitiesWithImages(List<Activity> activities)
        {
            try
            {
                // Map activities to DTOs
                var activitiesDto = _mapper.Map<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>(activities).ToList();
                
                // Fetch all images in one query
                var activityImages = await _context.ActivityImages
                    .Where(img => activities.Select(a => a.Id).Contains(img.ActivityId))
                    .ToListAsync();

                // Group images by ActivityId
                var imagesDict = activityImages
                    .GroupBy(img => img.ActivityId)
                    .ToDictionary(g => g.Key, g => g.Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToList());

                // Assign images to DTOs
                foreach (var activity in activitiesDto)
                {
                    activity.imageURLs = imagesDict.ContainsKey(activity.ActivityId) ? imagesDict[activity.ActivityId] : new List<string>();
                }

                // Return only activities that have images
                return activitiesDto.Where(a => a.imageURLs.Any()).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error mapping activities with images: {ex.Message}", ex);
            }
        }

        // Helper method to fetch and format reviews
        private async Task<List<Review>> GetFormattedReviews(int activityID)
        {
            try
            {
                var reviews = await _context.Reviews
                    .Where(r => r.ActivityId == activityID)
                    .Select(r => new Review
                    {
                        Id = r.Id,
                        ActivityId = r.ActivityId,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        ImageUrl = r.ImageUrl != null ? $"{_configuration["BaseUrl"]}{r.ImageUrl}" : null ,
                        ReviewTitle = r.ReviewTitle,
                        Date = r.Date ,
                        UserImageURl = r.UserImageURl,
                        CreatedAt = r.CreatedAt,
                        VisitorType = r.VisitorType,
                        USERNAME = r.USERNAME
                    })
                    .ToListAsync();

                return reviews;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching and formatting reviews: {ex.Message}", ex);
            }
        }
    }
}
