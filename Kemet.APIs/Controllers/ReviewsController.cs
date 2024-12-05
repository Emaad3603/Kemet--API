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

        public ReviewsController(IReviewRepository reviewRepo, IMapper mapper, UserManager<AppUser> userManager,AppDbContext context, IWebHostEnvironment environment)
        {
            _reviewRepo = reviewRepo;
            _mapper = mapper;
            _userManager = userManager;
           _context = context;
            _environment = environment;
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

                var activity = await _context.Activities.FindAsync(reviewDto.ActivityId);
                if (activity == null)
                    return NotFound("Activity not found.");

                string imageUrl = null;

                // Handle image upload
                if (reviewDto.Image != null)
                {
                    var fileHelper = new FileUploadHelper(_environment);
                    imageUrl = await fileHelper.SaveFileAsync(reviewDto.Image, "uploads/reviews");
                }

                var review = new Review
                {   UserId = user.Id,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    ActivityId = reviewDto.ActivityId,
                    ImageUrl = imageUrl,
                    PlaceId = reviewDto.PlaceId,
                    TravelAgencyPlanId = reviewDto.TravelAgencyPlanId
                };

                await _reviewRepo.AddAsync(review); // Use the repository to add the review
                return Ok("Review added successfully!");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("{activityId}/details")]
        public async Task<IActionResult>GetActivityWithReviews(int activityId) 
        {


            var activity = await _reviewRepo.GetReviewsForActivityAsync(activityId);
            if (activity == null)
                return NotFound("Activity reviews not found.");
           
            return Ok(activity);
        }
        [HttpGet("{placeId}/details")]
        public async Task<IActionResult> GetPlaceWithReviews(int placeid)
        {


            var Place = await _reviewRepo.GetReviewsForPlaceAsync(placeid);
            if (Place == null)
                return NotFound("place reviews not found.");

            return Ok(Place);
        }
        [HttpGet("{TravelAgencyPlanId}/details")]
        public async Task<IActionResult> GetReviewsForTravelAgencyPlan(int TravelAgencyPlanId)
        {


            var TravelAgencyPlan = await _reviewRepo.GetReviewsForTravelAgencyPlanAsync(TravelAgencyPlanId);
            if (TravelAgencyPlan == null)
                return NotFound("TravelAgencyPlan reviews not found.");

            return Ok(TravelAgencyPlan);
        }

        [HttpGet("place/{placeId}/average-rating")]
        public async Task<IActionResult> GetAverageRatingForPlace(int placeId)
        {
            var averageRating = await _reviewRepo.GetAverageRatingForPlaceAsync(placeId);
            return Ok(averageRating);
        }
        [HttpGet("activity/{activityId}/average-rating")]
        public async Task<IActionResult> GetAverageRatingForActivity(int activityId)
        {
            var averageRating = await _reviewRepo.GetAverageRatingForActivityAsync(activityId);
            return Ok(averageRating);
        }
        [HttpGet("travelagencyplan/{planId}/average-rating")]
        public async Task<IActionResult> GetAverageRatingForTravelAgencyPlan(int planId)
        {
            var averageRating = await _reviewRepo.GetAverageRatingForTravelAgencyPlanAsync(planId);
            return Ok(averageRating);
        }

    }

  

}



