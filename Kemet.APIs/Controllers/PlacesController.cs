using AutoMapper;
using Google.Type;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.APIs.Helpers;
using Kemet.Core.Entities;
using Kemet.Core.Entities.AI_Entites;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Core.Specifications;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.PlaceSpecs;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace Kemet.APIs.Controllers
{
  
    public class PlacesController : BaseApiController
    {
        private readonly IGenericRepository<Place> _placesRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IHomeServices _homeServices;
        private readonly ICacheRepository _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        //getAll
        //getByid
        public PlacesController(
            IGenericRepository<Place>placesRepo
            ,IMapper mapper
            ,AppDbContext context
            ,UserManager<AppUser> userManager
            ,IConfiguration configuration
            ,IHomeServices homeServices
            ,ICacheRepository cache
            )
        {
            _placesRepo = placesRepo;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _homeServices = homeServices;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlacesDto>>> GetPlaces()
        {
            try
            {
                var jsonOptions = JsonOptionsHelper.GetOptions();

                string cacheKey = "places_list";
                var cached = await _cache.GetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cached))
                    return JsonSerializer.Deserialize<List<PlacesDto>>(cached)!;

                var resultPlaces = await _homeServices.GetPlaces();
                             
                var places = _mapper.Map<IEnumerable<Place>, IEnumerable<PlacesDto>>(resultPlaces);

                foreach (var place in places.Take(10).ToList())
                {
                   var images = await _context.PlaceImages.Where(p => p.PlaceId == place.PlaceID)
                                                          .Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}")
                                                          .ToListAsync();
                   place.ImageURLs = images;
                }

                var result = places.Where(a => a.ImageURLs.Any()).ToList();
                var serialized = JsonSerializer.Serialize(result, jsonOptions);
                await _cache.SetAsync(cacheKey, serialized, _cacheDuration);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("GetPlaceByID")]
        public async Task<ActionResult<PlacesDto>> GetPlaceById(int PlaceID)
        {
            try
            {
                var spec = new PlaceWithCategoriesAndactivitiesSpecifications(PlaceID);
                var place = await _placesRepo.GetWithSpecAsync(spec);
                if (place == null)
                {
                    return NotFound(new ApiResponse(404, "No Places found "));
                }

                var Places = _mapper.Map<Place, DetailedPlaceDto>(place);
                if (Places == null)
                {
                    return NotFound(new ApiResponse(404, "No Places found "));
                }
                var fetchedPlaces = await _context.Reviews.Where(p => p.PlaceId == PlaceID).ToListAsync();
                foreach (var fetchedPlace in fetchedPlaces)
                {
                    fetchedPlace.Place = null;
                    fetchedPlace.ImageUrl = $"{_configuration["BaseUrl"]}{fetchedPlace.ImageUrl}";
                }
                Places.Reviews = fetchedPlaces;
                Places.CategoryName = await _context.Categories.Where(c => c.Id == place.CategoryId).Select(c => c.CategoryName).FirstOrDefaultAsync();
                var Result = Places;
                return Ok(Result);
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
                var places = await _context.Places.ToListAsync();
                var B7B7places = new List<B7B7DTO>();
                foreach (var place in places)
                {
                    var images = await _context.PlaceImages.Where(p => p.PlaceId == place.Id).Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").FirstOrDefaultAsync();               
                    place.PictureUrl = images;
                }
                foreach (var place in places)
                {
                    var test = new B7B7DTO();
                    test.PlaceId = place.Id;
                    test.CategoryName = await _context.Categories
                        .Where(c => c.Id == place.CategoryId)
                        .Select(c => c.CategoryName)
                        .FirstOrDefaultAsync();
                    test.Name = place.Name;
                    test.CulturalTips = place.CultureTips;
                    test.Address = await _context.Locations
                        .Where(l => l.Id == place.locationId)
                        .Select(l => l.Address)
                        .FirstOrDefaultAsync();
                    test.Duration = place.Duration;
                    test.CloseTime = place.CloseTime;
                    test.OpenTime = place.OpenTime;
                    test.Description = place.Description;
                    test.imageURLs = place.PictureUrl;
                    test.LocationLink = await _context.Locations
                        .Where(l => l.Id == place.locationId)
                        .Select(l => l.LocationLink)
                        .FirstOrDefaultAsync();
                    test.TouristAdult = await _context.Prices
                        .Where(p => p.Id == place.priceId)
                        .Select(p => p.TouristAdult)
                        .FirstOrDefaultAsync();
                    test.EgyptianAdult = await _context.Prices
                        .Where(p => p.Id == place.priceId)
                        .Select(p => p.EgyptianAdult)
                        .FirstOrDefaultAsync();

                    B7B7places.Add(test);
                }

                return Ok(B7B7places);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("PlacesHiddenGems")]
        public async Task<ActionResult<IEnumerable<PlacesDto>>> GetPlacesHiddenGems()
        {
            try
            {
                var jsonOptions = JsonOptionsHelper.GetOptions();

                string cacheKey = "hidden_places_list";
                var cached = await _cache.GetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cached))
                    return JsonSerializer.Deserialize<List<PlacesDto>>(cached)!;
                var resultPlaces = await _homeServices.GetPlacesHiddenGems();
                var result = await MapPlacesWithImages(resultPlaces);
                if (result == null) { return NotFound(new ApiResponse(404, "No places found within the maximum radius")); }
                var serialized = JsonSerializer.Serialize(result, jsonOptions);
                await _cache.SetAsync(cacheKey, serialized, _cacheDuration);
                return Ok(result);
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("placesTopRated")]
        public async Task<IActionResult> GetPlacesTopRated()
        {
            try 
            {
                var resultPlaces = await _homeServices.GetTopRatedPlaces();
                var result = await MapPlacesWithImages(resultPlaces);
                if (result == null) { return NotFound(new ApiResponse(404, "No places found within the maximum radius")); }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("TopAttractionsNearMe")]
        public async Task<IActionResult> GetTopAttractionsNearMe()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = new AppUser();
                if (userEmail != null)
                {
                    user = await _userManager.FindByEmailAsync(userEmail);
                }

                var resultPlaces = new List<Place>();

                if (user == null || user.Location == null)
                {
                    resultPlaces = await _homeServices.GetPlaces();
                }
                else
                {
                    var nearbyPlaces = await _homeServices.GetNearbyPlaces(user);
                    if (!nearbyPlaces.Any())
                    {
                        return NotFound(new ApiResponse(404, "No places found within the maximum radius."));
                    }
                    resultPlaces = nearbyPlaces.Take(10).ToList();
                }
                var places = await MapPlacesWithImages(resultPlaces);
                return Ok(places);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("PlacesInCairo")]
        public async Task<IActionResult> GetPlacesIncairo()
        {
            try
            {
                var jsonOptions = JsonOptionsHelper.GetOptions();

                string cacheKey = "Cairo_places_list";
                var cached = await _cache.GetAsync(cacheKey);

                if (!string.IsNullOrEmpty(cached))
                    return Ok(JsonSerializer.Deserialize<List<PlacesDto>>(cached)!);
                var resultPlaces = await _homeServices.GetPlacesInCairo();
                var result = await MapPlacesWithImages(resultPlaces);
                if (result == null) { return NotFound(new ApiResponse(404, "No places found within the maximum radius")); }
                var serialized = JsonSerializer.Serialize(result, jsonOptions);
                await _cache.SetAsync(cacheKey, serialized, _cacheDuration);
                return Ok(result);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet("PlacesForYou")]
        public async Task<IActionResult> GetPlacesByInterests()
        {
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                var user = new AppUser();
                if (userEmail != null)
                {
                    user = await _userManager.FindByEmailAsync(userEmail);
                }
                var places = await _homeServices.GetPlacesByCustomerInterests(user.Id);

                if (places == null || places.Count == 0)
                {
                    return NotFound();
                }
                var resPlaces = await MapPlacesWithImages(places);
                return Ok(resPlaces);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
                
        }

        private async Task<List<PlacesDto>> MapPlacesWithImages(List<Place> places) 
        {
            try
            {
                //map place to Dtos        
                var placesDto = _mapper.Map<IEnumerable<Place>, IEnumerable<PlacesDto>>(places).ToList();
                //fetch all images in one query

                var placeImages = await _context.PlaceImages
                     .Where(img => places.Select(a => a.Id).Contains(img.PlaceId))
                     .ToListAsync();
                var imagesDict = placeImages
                       .GroupBy(img => img.PlaceId)
                       .ToDictionary(g => g.Key, g => g.Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToList());
                //assign images to dtos
                foreach (var place in placesDto) 
                {
                    place.ImageURLs = imagesDict.ContainsKey(place.PlaceID) ? imagesDict[place.PlaceID] : new List<string>();
                }
                //return only places that have images
                return placesDto.Where(a => a.ImageURLs.Any()).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}


