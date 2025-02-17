using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.PlaceSpecs;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{
  
    public class PlacesController : BaseApiController
    {
        private readonly IGenericRepository<Place> _placesRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;


        //getAll
        //getByid
        public PlacesController(
            IGenericRepository<Place>placesRepo
            ,IMapper mapper
            ,AppDbContext context
            ,UserManager<AppUser> userManager
            ,IConfiguration configuration
            )
        {
            _placesRepo = placesRepo;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlacesDto>>> GetPlaces()
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
                List<Place> nearbyPlaces = new List<Place>();

                // Fetch places within the radius, increasing the radius until at least 5 places are found
                while (nearbyPlaces.Count < 10)
                {
                    // Fetch places within the current radius
                    var placesWithinRadius = await _context.Places
                        .Where(p => p.Location.Coordinates.Distance(userLocation) <= radius)
                        .ToListAsync();

                    // Add places to the nearbyPlaces list
                    nearbyPlaces.AddRange(placesWithinRadius);

                    // If no places are found, break the loop to avoid infinite looping
                    if (!placesWithinRadius.Any())
                    {
                        break;
                    }

                    // Increase the radius by 10 km
                    radius += 10000;
                }

                // If no places are found after increasing the radius, return a 404 response
                if (!nearbyPlaces.Any())
                {
                    return NotFound(new ApiResponse(404, "No places found within the maximum radius."));
                }

                // Take at least 5 places (or all available if less than 5)
                var resultPlaces = nearbyPlaces.Take(10).ToList();
                var places=  _mapper.Map<IEnumerable<Place>, IEnumerable<PlacesDto>>(resultPlaces);
                foreach(var place in places)
                {
                   var images =  await   _context.PlaceImages.Where(p => p.PlaceId == place.PlaceID).Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToListAsync();
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

                var Places = _mapper.Map<Place,DetailedPlaceDto>(place);
                if (Places == null)
                {
                    return NotFound(new ApiResponse(404, "No Places found "));
                }
                var fetchedPlaces = await _context.Reviews.Where(p => p.PlaceId == PlaceID).ToListAsync();
                foreach (var fetchedPlace in fetchedPlaces)
                {
                    fetchedPlace.ImageUrl = $"{_configuration["BaseUrl"]}{fetchedPlace.ImageUrl}";
                }
                Places.Reviews = fetchedPlaces;
                var Result = Places;
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
