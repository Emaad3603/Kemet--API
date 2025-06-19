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
using Kemet.APIs.DTOs.ReviewsDTOs;
using Kemet.Core.Repositories.InterFaces;

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
        private readonly FileUploadHelper _fileUploadHelper;
        private readonly IReviewRepository _reviewRepo;
        private readonly IBackgroundTaskQueue _queue;
        private readonly ILogger<AdminController> _logger;
        private readonly ICacheRepository _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public AdminController(AppDbContext context,
            IMapper mapper,
            UserManager<AppUser> userManager,
            IHomeServices homeServices,
            IConfiguration configuration,
            IGenericRepository<Place> placeRepository,
            IGenericRepository<Activity>activityRepository,
            IGenericRepository<Review>reviewRepository,
            IWebHostEnvironment environment,
            FileUploadHelper fileUploadHelper,
            IReviewRepository reviewRepo,
            IBackgroundTaskQueue queue ,
            ILogger<AdminController> logger ,
            ICacheRepository cache
            )
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
            _fileUploadHelper = fileUploadHelper;
            _reviewRepo = reviewRepo;
            _queue = queue;
            _logger = logger;
            _cache = cache;
        }

        [HttpPost("addplace")]
        public async Task<IActionResult> AddPlace([FromForm] AddPlaceDtos dto)
        {
            try
            {
                // Validate ImageURLs
                if (dto.ImageURLs == null || !dto.ImageURLs.Any())
                {
                    return BadRequest(new { 
                        message = "The ImageURLs field is required.",
                        errors = new { 
                            ImageURLs = new[] { "The ImageURLs field is required." } 
                        } 
                    });
                }
                
                // Check if price already exists
                var existingPrice = await _context.Prices.FirstOrDefaultAsync(p =>
                    p.EgyptianAdult == dto.EgyptianAdultCost &&
                    p.EgyptianStudent == dto.EgyptianStudentCost &&
                    p.TouristAdult == dto.TouristAdultCost &&
                    p.TouristStudent == dto.TouristStudentCost);

                Price price;
                if (existingPrice != null)
                {
                    price = existingPrice;
                }
                else
                {
                    // Find the max price ID and add 1 to ensure a unique ID
                    var maxPriceId = await _context.Prices.MaxAsync(p => (int?)p.Id) ?? 0;
                    
                    price = new Price
                    {
                        Id = maxPriceId + 1, // Explicitly set ID to avoid primary key violation
                        EgyptianAdult = dto.EgyptianAdultCost,
                        EgyptianStudent = dto.EgyptianStudentCost,
                        TouristAdult = dto.TouristAdultCost,
                        TouristStudent = dto.TouristStudentCost
                    };
                    
                    _context.Prices.Add(price);
                    await _context.SaveChangesAsync(); // Save to get the ID
                }

                // Map and set price
                var place = _mapper.Map<Place>(dto);
                place.Price = price;
                place.CultureTips = dto.CulturalTips;
                place.OpenTime = dto.OpenTime;
                place.CloseTime = dto.CloseTime;
                // Validate and set category
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName && c.CategoryType == "place");

                if (category == null)
                {
                    return BadRequest(new { message = "Invalid CategoryName or CategoryType" });
                }

                place.Category = category;
                place.CategoryId = category.Id;

                // Add place first to get its ID
                await _placeRepository.AddAsync(place);
                await _context.SaveChangesAsync();

                // Handle image uploads
                var savedImageUrls = new List<string>();
                var directoryPath = Path.Combine(_environment.WebRootPath, "images", "places");

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                foreach (var image in dto.ImageURLs)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(directoryPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var placeImage = new PlaceImage
                        {
                            ImageUrl = $"/images/places/{fileName}",
                            PlaceId = place.Id
                        };

                        _context.PlaceImages.Add(placeImage);
                        savedImageUrls.Add(placeImage.ImageUrl);
                    }
                }

                await _context.SaveChangesAsync();
                UpdateModelData();
                return Ok(new
                {
                    message = "Place added successfully",
                    data = new
                    {
                        place.Id,
                        place.Name,
                        place.Description,
                        Images = savedImageUrls
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        //  Update Place
        [HttpPut("editplace/{id}")]
        public async Task<IActionResult> EditPlace(int id, [FromForm] AddPlaceDtos dto)
        {
            try
            {
                var existingPlace = await _context.Places
                    .Include(p => p.Images)
                    .Include(p => p.Price)
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingPlace == null)
                    return NotFound(new { message = "Place not found" });

                // Update basic fields
                existingPlace.Name = dto.Name;
                existingPlace.Description = dto.Description;
                existingPlace.CultureTips = dto.CulturalTips;
                existingPlace.Duration = dto.Duration;

                // Remove existing images
                _context.PlaceImages.RemoveRange(existingPlace.Images);
                var newImages = new List<PlaceImage>();

                // Handle image uploads
                var directoryPath = Path.Combine(_environment.WebRootPath, "images", "places");

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                if (dto.ImageURLs != null && dto.ImageURLs.Any())
                {
                    foreach (var file in dto.ImageURLs)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(directoryPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            newImages.Add(new PlaceImage
                            {
                                ImageUrl = $"/images/places/{fileName}",
                                PlaceId = id
                            });
                        }
                    }

                    existingPlace.Images = newImages;
                }

                // Handle price
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
                    // Find the max price ID and add 1 to ensure a unique ID
                    var maxPriceId = await _context.Prices.MaxAsync(p => (int?)p.Id) ?? 0;
                    
                    var newPrice = new Price
                    {
                        Id = maxPriceId + 1, // Explicitly set ID to avoid primary key violation
                        EgyptianAdult = dto.EgyptianAdultCost,
                        EgyptianStudent = dto.EgyptianStudentCost,
                        TouristAdult = dto.TouristAdultCost,
                        TouristStudent = dto.TouristStudentCost
                    };
                    await _context.Prices.AddAsync(newPrice);
                    existingPlace.Price = newPrice;
                }

                // Handle category
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName && c.CategoryType == "place");

                if (existingCategory != null)
                {
                    existingPlace.Category = existingCategory;
                    existingPlace.CategoryId = existingCategory.Id;
                }
                else
                {
                    var newCategory = new Category
                    {
                        CategoryName = dto.CategoryName,
                        CategoryType = "place"
                    };
                    await _context.Categories.AddAsync(newCategory);
                    existingPlace.Category = newCategory;
                    existingPlace.CategoryId = newCategory.Id;
                }
                existingPlace.OpenTime = dto.OpenTime;
                existingPlace.CloseTime = dto.CloseTime;
                await _context.SaveChangesAsync();
                UpdateModelData();
                await PlacesInvalidateCacheAsync();
                return Ok(new
                {
                    message = "Place updated successfully",
                    data = new
                    {
                        existingPlace.Id,
                        existingPlace.Name,
                        existingPlace.Description,
                        existingPlace.CultureTips,
                        existingPlace.Duration,
                        Images = existingPlace.Images.Select(i => i.ImageUrl).ToList()
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }


        [HttpDelete("DeletePlace/{id}")]
        public async Task<IActionResult> DeletePlace(int id)
        {
            try
            {
                var place = await _context.Places
                    .Include(p => p.Images)
                    .Include(p => p.Reviews)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (place == null)
                    return NotFound(new ApiResponse(404, "Place not found"));

               
                if (place.Reviews.Any())
                    _context.Reviews.RemoveRange(place.Reviews);

               
                if (place.Images.Any())
                {
               

                    _context.PlaceImages.RemoveRange(place.Images);
                }

                _context.Places.Remove(place);
                await _context.SaveChangesAsync();
                UpdateModelData();
                await PlacesInvalidateCacheAsync();
                return Ok(new { message = "Place and all related data deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
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
        public async Task<IActionResult> AddActivity([FromForm] AddActivityDto activityDto)
        {
            try
            {
                if (activityDto.ImageURLs == null || !activityDto.ImageURLs.Any())
                {
                    return BadRequest(new { 
                        message = "The ImageURLs field is required.",
                        errors = new { 
                            ImageURLs = new[] { "The ImageURLs field is required." } 
                        } 
                    });
                }

                // First, create and save the category if needed
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == activityDto.CategoryName && c.CategoryType == "activity");

                if (category == null)
                {
                    category = new Category
                    {
                        CategoryName = activityDto.CategoryName,
                        CategoryType = "activity"
                    };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync(); // Save to get the ID
                }

                // Handle the price separately to avoid PK issues
                Price price;
                var existingPrice = await _context.Prices.FirstOrDefaultAsync(p =>
                    p.EgyptianAdult == activityDto.EgyptianAdultCost &&
                    p.EgyptianStudent == activityDto.EgyptianStudentCost &&
                    p.TouristAdult == activityDto.TouristAdultCost &&
                    p.TouristStudent == activityDto.TouristStudentCost);

                if (existingPrice != null)
                {
                    price = existingPrice;
                }
                else
                {
                    // Find the max price ID and add 1 to ensure a unique ID
                    var maxPriceId = await _context.Prices.MaxAsync(p => (int?)p.Id) ?? 0;
                    
                    price = new Price
                    {
                        Id = maxPriceId + 1, // Explicitly set ID to avoid primary key violation
                        EgyptianAdult = activityDto.EgyptianAdultCost,
                        EgyptianStudent = activityDto.EgyptianStudentCost,
                        TouristAdult = activityDto.TouristAdultCost,
                        TouristStudent = activityDto.TouristStudentCost
                    };
                    
                    // This ensures the EF Core won't try to insert with ID = 0
                    _context.Prices.Add(price);
                    await _context.SaveChangesAsync(); // Save to get the ID
                }

                // Create the activity using the IDs we now have
                var activity = new Activity
                {
                    Name = activityDto.Name,
                    Description = activityDto.Description,
                    Duration = activityDto.Duration,
                    CulturalTips = activityDto.CulturalTips,
                    priceId = price.Id,
                    CategoryId = category.Id,
                    AverageRating = 0,
                    RatingsCount = 0,
                    CloseTime = activityDto.CloseTime ,
                    OpenTime = activityDto.OpenTime ,  // Default value, adjust as needed
                    GroupSize = 10     
                    
                    // Default value, adjust as needed
                };
                
                _context.Activities.Add(activity);
                await _context.SaveChangesAsync();

                // Handle image uploads
                var savedImageUrls = new List<string>();
                var directoryPath = Path.Combine(_environment.WebRootPath, "images", "activities");

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                foreach (var image in activityDto.ImageURLs)
                {
                    if (image.Length > 0)
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                        var filePath = Path.Combine(directoryPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        var activityImage = new ActivityImage
                        {
                            ImageUrl = $"/images/activities/{fileName}",
                            ActivityId = activity.Id
                        };

                        _context.ActivityImages.Add(activityImage);
                        savedImageUrls.Add(activityImage.ImageUrl);
                    }
                }

                await _context.SaveChangesAsync();
                UpdateModelData();
                return Ok(new
                {
                    message = "Activity added successfully",
                    data = new
                    {
                        activity.Id,
                        activity.Name,
                        activity.Description,
                        activity.Duration,
                        price.EgyptianAdult,
                        price.TouristAdult,
                        CategoryName = category.CategoryName,
                        Images = savedImageUrls
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("editactivity/{id}")]
        public async Task<IActionResult> EditActivity(int id, [FromForm] AddActivityDto dto)
        {
            try
            {
                var existingActivity = await _context.Activities
                    .Include(a => a.Images)
                    .Include(a => a.Price)
                    .Include(a => a.Category)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (existingActivity == null)
                    return NotFound(new { message = "Activity not found" });

                // Update basic fields
                existingActivity.Name = dto.Name;
                existingActivity.Description = dto.Description;
                existingActivity.CulturalTips = dto.CulturalTips;
                existingActivity.Duration = dto.Duration;

                // Remove existing images
                _context.ActivityImages.RemoveRange(existingActivity.Images);
                var newImages = new List<ActivityImage>();

                // Handle image uploads
                var directoryPath = Path.Combine(_environment.WebRootPath, "images", "activities");

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                if (dto.ImageURLs != null && dto.ImageURLs.Count > 0)
                {
                    foreach (var file in dto.ImageURLs)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(directoryPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            newImages.Add(new ActivityImage
                            {
                                ImageUrl = $"/images/activities/{fileName}",
                                ActivityId = id
                            });
                        }
                    }

                    existingActivity.Images = newImages;
                }

                // Handle price
                var existingPrice = await _context.Prices.FirstOrDefaultAsync(p =>
                    p.EgyptianAdult == dto.EgyptianAdultCost &&
                    p.EgyptianStudent == dto.EgyptianStudentCost &&
                    p.TouristAdult == dto.TouristAdultCost &&
                    p.TouristStudent == dto.TouristStudentCost);

                if (existingPrice != null)
                {
                    existingActivity.Price = existingPrice;
                }
                else
                {
                    // Find the max price ID and add 1 to ensure a unique ID
                    var maxPriceId = await _context.Prices.MaxAsync(p => (int?)p.Id) ?? 0;
                    
                    var newPrice = new Price
                    {
                        Id = maxPriceId + 1, // Explicitly set ID to avoid primary key violation
                        EgyptianAdult = dto.EgyptianAdultCost,
                        EgyptianStudent = dto.EgyptianStudentCost,
                        TouristAdult = dto.TouristAdultCost,
                        TouristStudent = dto.TouristStudentCost
                    };
                    await _context.Prices.AddAsync(newPrice);
                    existingActivity.Price = newPrice;
                }

                // Handle category (reuse or create)
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName && c.CategoryType == "activity");

                if (existingCategory != null)
                {
                    existingActivity.Category = existingCategory;
                }
                else
                {
                    var newCategory = new Category
                    {
                        CategoryName = dto.CategoryName,
                        CategoryType = "activity"
                    };
                    await _context.Categories.AddAsync(newCategory);
                    existingActivity.Category = newCategory;
                }

                await _context.SaveChangesAsync();
                UpdateModelData();
                await ActivitiesInvalidateCacheAsync();
                return Ok(new
                {
                    message = "Activity updated successfully",
                    data = new
                    {
                        existingActivity.Id,
                        existingActivity.Name,
                        existingActivity.Description,
                        existingActivity.CulturalTips,
                        existingActivity.Duration,
                        Price = existingActivity.Price,
                        Images = existingActivity.Images.Select(i => i.ImageUrl).ToList(),
                        Category = existingActivity.Category?.CategoryName
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteActivity/{id}")]
        public async Task<IActionResult> DeleteActivity(int id)
        {
            try
            {
                var activity = await _context.Activities
                    .Include(p => p.Images)
                    .Include(p => p.Reviews)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (activity == null)
                    return NotFound(new ApiResponse(404, "Activity not found"));


                if (activity.Reviews.Any())
                    _context.Reviews.RemoveRange(activity.Reviews);


                if (activity.Images.Any())
                {
                   

                    _context.ActivityImages.RemoveRange(activity.Images);
                }

                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
                UpdateModelData();
                await ActivitiesInvalidateCacheAsync();

                return Ok(new { message = "Activity and all related data deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
        [HttpGet("GetAllActivities")]
        public async Task<ActionResult<ActivityDTOs>> GetActivites()
        {
            try
            {
                var resultActivities = await _homeServices.GetActivities();
                var result = await MapActivitiesWithImages(resultActivities);

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
        [HttpGet("GetAllCustomers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                
                var customers = await _userManager.GetUsersInRoleAsync("Customer");

               
                var userDtos = customers.Select(user => new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    ImageURL = $"{_configuration["BaseUrl"]}{user.ImageURL}"
                }).ToList();

                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
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
                    ProfileURl =  $"{_configuration["BaseUrl"]}{agency.ImageURL}",        
                    BackgroundURL = $"{_configuration["BaseUrl"]}{agency.BackgroundImageURL}",   
                    
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
        [HttpPost("AddTravelAgency")]
        public async Task<IActionResult> AddTravelAgency([FromBody] TravelAgencyRegisterDTO dto)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
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
                throw new Exception($"Error mapping activities with images: {ex.Message}");
            }
        }

        [HttpGet("reviews")]
        public async Task<ActionResult<IEnumerable<AdminReviewDto>>> GetAllReviews()
        {
            try
            {
                var reviews = await _reviewRepo.GetAllReviewsForAdminAsync();
                if (reviews == null || !reviews.Any())
                    return Ok(new List<AdminReviewDto>());

                var reviewsToReturn = new List<AdminReviewDto>();

                foreach (var review in reviews)
                {
                    var reviewDto = new AdminReviewDto
                    {
                        Id = review.Id,
                        UserId = review.UserId,
                        UserName = review.USERNAME,
                        Date = review.Date,
                        ReviewTitle = review.ReviewTitle,
                        VisitorType = review.VisitorType,
                        UserImageURL = review.UserImageURl,
                        Comment = review.Comment,
                        Rating = review.Rating,
                        ImageUrl = review.ImageUrl,
                        CreatedAt = review.CreatedAt,
                        ActivityId = review.ActivityId,
                        PlaceId = review.PlaceId,
                        TravelAgencyPlanId = review.TravelAgencyPlanId,
                        TravelAgencyId = review.TravelAgencyID
                    };

                    // Set review type and item name
                    if (review.PlaceId != null)
                    {
                        reviewDto.ReviewType = "Place";
                        reviewDto.ItemName = review.Place?.Name ?? "Unknown Place";
                    }
                    else if (review.ActivityId != null)
                    {
                        reviewDto.ReviewType = "Activity";
                        reviewDto.ItemName = review.Activity?.Name ?? "Unknown Activity";
                    }
                    else if (review.TravelAgencyPlanId != null)
                    {
                        reviewDto.ReviewType = "TravelAgencyPlan";
                        reviewDto.ItemName = review.TravelAgencyPlan?.PlanName ?? "Unknown Plan";
                    }
                    else if (review.TravelAgencyID != null)
                    {
                        reviewDto.ReviewType = "TravelAgency";
                        reviewDto.ItemName = "Travel Agency"; // You might need to fetch the agency name
                    }

                    reviewsToReturn.Add(reviewDto);
                }

                return Ok(reviewsToReturn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("DeleteReview/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            try
            {
                // Find the review by ID
                var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
                if (review == null)
                {
                    return NotFound(new { message = "Review not found" });
                }

                // Delete the associated image if it exists
                if (!string.IsNullOrEmpty(review.ImageUrl))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, review.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Remove the review from the database
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Review deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        private void UpdateModelData()
        {
            _queue.QueueBackgroundWorkItem(async ct =>
            {
                try
                {
                    using var client = new HttpClient();
                    var requestUri = "https://web-production-bbbd2.up.railway.app/api/sync-from-dotnet";

                    // POST with empty body
                    await client.PostAsync(requestUri, new StringContent(string.Empty, System.Text.Encoding.UTF8, "application/json"), ct)
                                .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    // Optional: log error so you know if the background call fails
                    _logger.LogError(ex, "Fire-and-forget POST failed.");
                }
            });
        }

        private async Task PlacesInvalidateCacheAsync()
        {
            await _cache.RemoveAsync("places_list");
            await _cache.RemoveAsync("hidden_places_list");
            await _cache.RemoveAsync("Cairo_places_list");
        }
        private async Task ActivitiesInvalidateCacheAsync()
        {
            await _cache.RemoveAsync("Activities_list");
            await _cache.RemoveAsync("Cairo_Activities_list");
            await _cache.RemoveAsync("Hidden_Activities_list");
        }
       
    }

}
