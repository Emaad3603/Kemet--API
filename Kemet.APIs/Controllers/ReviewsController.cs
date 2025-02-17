using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.ReviewsDTOs;
using Kemet.APIs.Errors;
using Kemet.APIs.Helpers;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.PlaceSpecs;
using Kemet.Core.Specifications.ReviewSpecs;
using Kemet.Repository.Data;
using Kemet.Repository.Repositories;
using Kemet.Repository.Specification;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{

    public class ReviewsController : BaseApiController
    {
        private readonly IReviewRepository _reviewRepo;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ReviewsController(IReviewRepository reviewRepo
            , IMapper mapper
            , UserManager<AppUser> userManager
            ,AppDbContext context
            , IWebHostEnvironment environment
            , IConfiguration configuration)
        {
            _reviewRepo = reviewRepo;
            _mapper = mapper;
            _userManager = userManager;
           _context = context;
            _environment = environment;
            _configuration = configuration;
        }


        [HttpPost]
        public async Task<IActionResult> AddReview([FromForm] ReviewDto reviewDto)
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

                var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
                if (user == null) return NotFound("User not found.");

                if (reviewDto == null)
                    return BadRequest("Invalid review data.");

          

                string imageUrl = null;
               

                // Handle image upload
                if (reviewDto.Image != null)
                {
                    var fileHelper = new FileUploadHelper(_environment);
                    imageUrl = await fileHelper.SaveFileAsync(reviewDto.Image, "uploads/reviews");
                }

                var review = new Review
                {   UserId = user.Id,
                    USERNAME = user.UserName,
                    UserImageURl = $"{_configuration["BaseUrl"]}{user.ImageURL}",
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    ActivityId = reviewDto.ActivityId,
                    ImageUrl = imageUrl,
                    PlaceId = reviewDto.PlaceId,
                    TravelAgencyPlanId = reviewDto.TravelAgencyPlanId
                };

               await _reviewRepo.AddReviewAsync(review);

            
                return Ok("Review added successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

       [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview(int reviewId, [FromForm] ReviewDto reviewDto)
        {
            try
            {
                var oldReview = await _reviewRepo.GetAsync(reviewId);
                if (oldReview == null)
                {
                    return NotFound("Review not found.");
                }

                if (reviewDto == null)
                {
                    return BadRequest("Invalid review data.");
                }

                // Update review fields
                oldReview.Rating = reviewDto.Rating;
                oldReview.Comment = reviewDto.Comment;

                _reviewRepo.Update(oldReview); // Use your repository to update the review

                // Update AverageRating and RatingsCount for the associated entity
                if (oldReview.PlaceId != null)
                {
                    var entity = await _context.Places.FindAsync(oldReview.PlaceId); // Similar for Activity/Plan
                    if (entity != null)
                    {
                        var totalRating = entity.AverageRating * entity.RatingsCount;
                        totalRating = totalRating - oldReview.Rating + reviewDto.Rating;
                        entity.AverageRating = totalRating / entity.RatingsCount;

                        _context.Places.Update(entity);
                        await _context.SaveChangesAsync();
                    }
                }

                if (oldReview.ActivityId != null)
                {
                    var activity = await _context.Activities.FindAsync(oldReview.ActivityId);
                    if (activity != null)
                    {
                        var totalRating = activity.AverageRating * activity.RatingsCount;
                        totalRating = totalRating - oldReview.Rating + reviewDto.Rating;
                        activity.AverageRating = totalRating / activity.RatingsCount;

                        _context.Activities.Update(activity);
                        await _context.SaveChangesAsync();
                    }
                }

                if (oldReview.TravelAgencyPlanId != null)
                {
                    var plan = await _context.TravelAgencyPlans.FindAsync(oldReview.TravelAgencyPlanId);
                    if (plan != null)
                    {
                        var totalRating = plan.AverageRating * plan.RatingsCount;
                        totalRating = totalRating - oldReview.Rating + reviewDto.Rating;
                        plan.AverageRating = totalRating / plan.RatingsCount;

                        _context.TravelAgencyPlans.Update(plan);
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok("Review updated successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        [HttpGet("activity/details/{activityId}")]
        public async Task<IActionResult>GetActivityWithReviews(int activityId) 
        {
            
                var activity = await _reviewRepo.GetReviewsForActivityAsync(activityId);
                if (activity == null) return NotFound("activity reviews not found.");




            return Ok(activity);
          

            
        }
        [HttpGet("place/details/{placeId}")]
        public async Task<IActionResult> GetPlaceWithReviews(int placeid)
        {


            var place = await _reviewRepo.GetReviewsForPlaceAsync(placeid);
            if (place == null)
                return NotFound("place reviews not found.");

            return Ok(place);
        }
        [HttpGet("travelagencyplan/details/{TravelAgencyPlanId}")]
        public async Task<IActionResult> GetReviewsForTravelAgencyPlan(int TravelAgencyPlanId)
        {


            var TravelAgencyPlan = await _reviewRepo.GetReviewsForTravelAgencyPlanAsync(TravelAgencyPlanId);
            if (TravelAgencyPlan == null)
                return NotFound("TravelAgencyPlan reviews not found.");

            return Ok(TravelAgencyPlan);
        }

        [HttpGet("place/average-rating/{placeId}")]
        public async Task<IActionResult> GetAverageRatingForPlace(int placeId)
        {
            var averageRating = await _reviewRepo.GetAverageRatingForPlaceAsync(placeId);
            return Ok(averageRating);
        }
        [HttpGet("activity/average-rating/{activityId}")]
        public async Task<IActionResult> GetAverageRatingForActivity(int activityId)
        {
            var averageRating = await _reviewRepo.GetAverageRatingForActivityAsync(activityId);
            return Ok(averageRating);
        }
        [HttpGet("travelagencyplan/average-rating/{planId}")]
        public async Task<IActionResult> GetAverageRatingForTravelAgencyPlan(int planId)
        {
            var averageRating = await _reviewRepo.GetAverageRatingForTravelAgencyPlanAsync(planId);
            return Ok(averageRating);
        }

    }

  

}



