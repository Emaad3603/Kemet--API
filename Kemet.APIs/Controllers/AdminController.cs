using AutoMapper;
using Kemet.Core.Entities.AI_Entites;
using Kemet.Core.Entities;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Kemet.APIs.Errors;
using Microsoft.EntityFrameworkCore;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Services.Interfaces;
using Kemet.APIs.Helpers;
using Kemet.Core.Entities.Images;
using System.Security.Claims;
using Kemet.APIs.DTOs.IdentityDTOs;
using Kemet.APIs.DTOs.ProfileDTOs;

namespace Kemet.APIs.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class AdminController : BaseApiController
    {
      
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHomeServices _homeServices;
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<Place> _placeRepository;
        private readonly IGenericRepository<Activity> _activityRepository;
        private readonly IGenericRepository<Review> _reviewRepository;
        private readonly IWebHostEnvironment _environment;

        public AdminController(AppDbContext context,
            IMapper mapper,
            UserManager<AppUser> userManager,
            IHomeServices homeServices,
            IConfiguration configuration,
            IGenericRepository<Place> placeRepository,
            IGenericRepository<Activity>activityRepository,
            IGenericRepository<Review>reviewRepository,
            IWebHostEnvironment environment)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _homeServices = homeServices;
            _configuration = configuration;
            _placeRepository = placeRepository;
           _activityRepository = activityRepository;
            _reviewRepository = reviewRepository;
            _environment = environment;
        }

        /* [HttpPost("addplace")]
         public async Task<IActionResult> AddPlace([FromBody] AddPlaceDtos dto)
         {
             try
             {
                 var place = _mapper.Map<Place>(dto);
                 await _placeRepository.AddAsync(place);
                 await _context.SaveChangesAsync();

                 return Ok(new { message = "Place added successfully", data = place });
             }
             catch (Exception ex)
             {
                 return StatusCode(500, new
                 {
                     StatusCode = 500,
                     Message = ex.InnerException?.Message ?? ex.Message
                 });
             }
         }*/
        [HttpPost("addplace")]
        public async Task<IActionResult> AddPlace([FromBody] AddPlaceDtos dto)
        {
            try
            {
                var existingPrice = await _context.Prices.FirstOrDefaultAsync(p =>
                    p.EgyptianAdult == dto.EgyptianAdultCost &&
                    p.EgyptianStudent == dto.EgyptianStudentCost &&
                    p.TouristAdult == dto.TouristAdultCost &&
                    p.TouristStudent == dto.TouristStudentCost);

                var price = existingPrice ?? new Price
                {
                    Id = new Random().Next(100000, int.MaxValue), 
                    EgyptianAdult = dto.EgyptianAdultCost,
                    EgyptianStudent = dto.EgyptianStudentCost,
                    TouristAdult = dto.TouristAdultCost,
                    TouristStudent = dto.TouristStudentCost
                };

                var place = _mapper.Map<Place>(dto);
                place.Price = price;

                await _placeRepository.AddAsync(place);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Place added successfully", data = place });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        //=============================================================================

        //=============================================================================
       
        //  Update Place
        [HttpPut("editplace/{id}")]
        public async Task<IActionResult> EditPlace(int id, [FromBody] AddPlaceDtos dto)
        {
            try
            {
                var existingPlace = await _context.Places
                    .Include(p => p.Images)
                    .Include(p => p.Price)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);  // Use 'Id'

                if (existingPlace == null)
                    return NotFound(new { message = "Place not found" });

                // Basic updates
                existingPlace.Name = dto.Name;
                existingPlace.Description = dto.Description;
                existingPlace.CultureTips = dto.CultureTips;
                existingPlace.Duration = dto.Duration;

                // Replace images
                _context.PlaceImages.RemoveRange(existingPlace.Images);
                existingPlace.Images = dto.ImageURLs
                    .Select(url => new PlaceImage { ImageUrl = url, PlaceId = id })
                    .ToList();

                // Handle Price
                var existingPrice = await _context.Prices.FirstOrDefaultAsync(p =>
                    p.EgyptianAdult == dto.EgyptianAdultCost &&
                    p.EgyptianStudent == dto.EgyptianStudentCost &&
                    p.TouristAdult == dto.TouristAdultCost &&
                    p.TouristStudent == dto.TouristStudentCost);

                if (existingPrice != null)
                {
                    existingPlace.Price = existingPrice;
                }
                else
                {
                    var newPrice = new Price
                    {
                        EgyptianAdult = dto.EgyptianAdultCost,
                        EgyptianStudent = dto.EgyptianStudentCost,
                        TouristAdult = dto.TouristAdultCost,
                        TouristStudent = dto.TouristStudentCost
                    };
                    await _context.Prices.AddAsync(newPrice);
                    existingPlace.Price = newPrice;
                }

                // Handle Category
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName);

                if (existingCategory != null)
                {
                    existingPlace.Category = existingCategory;
                }
                else
                {
                    var newCategory = new Category
                    {
                        CategoryName = dto.CategoryName,
                        CategoryType = "Historical"
                    };
                    await _context.Categories.AddAsync(newCategory);
                    existingPlace.Category = newCategory;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Place updated successfully", data = existingPlace });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        //  Delete Place
        [HttpDelete("DeletePlace/{id}")]
        public async Task<IActionResult> DeletePlace(int id)
        {
            var place = await _context.Places.FindAsync(id);
            if (place == null)
                return NotFound(new ApiResponse(404, "Place not found"));

            _context.Places.Remove(place);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Place deleted successfully" });
        }

        //  Get All Places (Admin View)
        [HttpGet("GetAllPlaces")]
        public async Task<ActionResult<IEnumerable<PlacesDto>>> GetPlaces()
        {
            try
            {


                var resultPlaces = await _homeServices.GetPlaces();

                var places = _mapper.Map<IEnumerable<Place>, IEnumerable<PlacesDto>>(resultPlaces);

                foreach (var place in places)
                {
                    var images = await _context.PlaceImages.Where(p => p.PlaceId == place.PlaceID)
                                                              .Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}")
                                                              .ToListAsync();
                    place.ImageURLs = images;
                }

                var result = places.Where(a => a.ImageURLs.Any()).ToList();

                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost("AddActivity")]
        public async Task<IActionResult> AddActivity([FromBody] AddActivityDto activityDto)
        {
            try
            {
                var existingPrice = await _context.Prices.FirstOrDefaultAsync(p =>
                    p.EgyptianAdult == activityDto.EgyptianAdultCost &&
                    p.EgyptianStudent == activityDto.EgyptianStudentCost &&
                    p.TouristAdult == activityDto.TouristAdultCost &&
                    p.TouristStudent == activityDto.TouristStudentCost);

                var price = existingPrice ?? new Price
                {
                    EgyptianAdult = activityDto.EgyptianAdultCost,
                    EgyptianStudent = activityDto.EgyptianStudentCost,
                    TouristAdult = activityDto.TouristAdultCost,
                    TouristStudent = activityDto.TouristStudentCost
                };

                var activity = _mapper.Map<Activity>(activityDto);
                activity.Price = price;

                await _context.Activities.AddAsync(activity);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Activity added successfully", data = activity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        [HttpPut("EditActivity/{id}")]
        public async Task<IActionResult> EditActivity(int id, [FromBody] AddActivityDto updatedActivity)
        {
            try
            {
                var existingActivity = await _context.Activities
                    .Include(a => a.Images)
                    .Include(a => a.Price)
                    .Include(a => a.Category)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (existingActivity == null)
                    return NotFound(new ApiResponse(404, "Activity not found"));

                // Update the basic fields
                _mapper.Map(updatedActivity, existingActivity);

                // Handle Image Updates (Similar to how you handled images in Place)
                if (updatedActivity.ImageURLs != null && updatedActivity.ImageURLs.Any())
                {
                    _context.ActivityImages.RemoveRange(existingActivity.Images);
                    existingActivity.Images = updatedActivity.ImageURLs
                        .Select(url => new ActivityImage { ImageUrl = url, ActivityId = id })
                        .ToList();
                }

                // Handle Price (Check if it needs to be updated or reused)
                var existingPrice = await _context.Prices.FirstOrDefaultAsync(p =>
                    p.EgyptianAdult == updatedActivity.EgyptianAdultCost &&
                    p.EgyptianStudent == updatedActivity.EgyptianStudentCost &&
                    p.TouristAdult == updatedActivity.TouristAdultCost &&
                    p.TouristStudent == updatedActivity.TouristStudentCost);

                if (existingPrice != null)
                {
                    existingActivity.Price = existingPrice;
                }
                else
                {
                    var newPrice = new Price
                    {
                        EgyptianAdult = updatedActivity.EgyptianAdultCost,
                        EgyptianStudent = updatedActivity.EgyptianStudentCost,
                        TouristAdult = updatedActivity.TouristAdultCost,
                        TouristStudent = updatedActivity.TouristStudentCost
                    };
                    await _context.Prices.AddAsync(newPrice);
                    existingActivity.Price = newPrice;
                }

                // Save changes
                await _context.SaveChangesAsync();

                return Ok(new { message = "Activity updated successfully", data = existingActivity });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


        [HttpDelete("DeleteActivity/{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
                return NotFound(new ApiResponse(404,"Activity not found."));

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Activity deleted successfully." });
        }
        //======================================================================

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();


                var userDtos = users.Select(user => new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,



                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet("GetAllTravelAgencies")]
        public async Task<IActionResult> GetAllTravelAgencies()
        {
            try
            {
                var travelAgencies = await _userManager.Users
                    .OfType<TravelAgency>()
                    
                    .ToListAsync();

                var result = travelAgencies.Select(agency => new TravelAgencyProfileDto
                {
                    UserName = agency.UserName,
                    Email = agency.Email,
                    PhoneNumber = agency.PhoneNumber,
                    Address = agency.Address,
                    Description = agency.Description,
                    FacebookURL = agency.FacebookURL,
                    InstagramURL = agency.InstagramURL,
                    Bio = agency.Bio,
                    ProfileURl = agency.ImageURL,        
                    BackgroundURL = agency.BackgroundImageURL,   
                    
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        [HttpPost("AddTravelAgency")]
        public async Task<IActionResult> AddTravelAgency([FromBody] TravelAgencyRegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email is already in use." });

            var travelAgency = new TravelAgency
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                Description = dto.Description,
                TaxNumber = dto.TaxNumber,
                FacebookURL = dto.FacebookURL,
                InstagramURL = dto.InstagramURL,
                Bio = dto.Bio,
               
            };

            var result = await _userManager.CreateAsync(travelAgency, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Optional: Assign a role like "TravelAgency"
            await _userManager.AddToRoleAsync(travelAgency, "TravelAgency");

            return Ok(new { message = "Travel agency added successfully." });
        }

        //[HttpGet("all-reviews")]
        //public async Task<IActionResult> GetAllReviewsWithType()
        //{
        //    var reviews = await _context.Reviews
        //        .Include(r => r.Place)
        //        .Include(r => r.Activity)
        //        .Include(r => r.TravelAgencyPlan)
        //        .ToListAsync();

        //    var mappedReviews = reviews.Select(r => new
        //    {
        //        r.Id,
        //        r.ReviewTitle,
        //        r.Comment,
        //        r.Rating,
        //        r.Date,
        //        r.USERNAME,
        //        r.UserImageURl,
        //        r.VisitorType,
        //        Type = r.PlaceId != null ? "Place"
        //              : r.ActivityId != null ? "Activity"
        //              : r.TravelAgencyPlanId != null ? "TravelAgencyPlan"
        //              : "Unknown",
        //        EntityName = r.Place != null ? r.Place.Name
        //                    : r.Activity != null ? r.Activity.Name
        //                    : r.TravelAgencyPlan != null ? r.TravelAgencyPlan.PlanName
        //                    : null
        //    });

        //    return Ok(mappedReviews);
        //}

    }
}
