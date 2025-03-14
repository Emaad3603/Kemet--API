using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
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
        private readonly IHomeServices _homeServices;


        //getAll
        //getByid
        public PlacesController(
            IGenericRepository<Place>placesRepo
            ,IMapper mapper
            ,AppDbContext context
            ,UserManager<AppUser> userManager
            ,IConfiguration configuration
            ,IHomeServices homeServices
            )
        {
            _placesRepo = placesRepo;
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
            _homeServices = homeServices;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlacesDto>>> GetPlaces()
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
                  resultPlaces =   await  _homeServices.GetPlaces();
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
                var places=  _mapper.Map<IEnumerable<Place>, IEnumerable<PlacesDto>>(resultPlaces);

                foreach(var place in places)
                {
                   var images =  await   _context.PlaceImages.Where(p => p.PlaceId == place.PlaceID)
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
                    fetchedPlace.Place = null;
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
        [HttpGet("B7B7")]
        public async Task<IActionResult> B7B7()
        {
            var places = await  _context.Places.ToListAsync();
            var B7B7places = new List<B7B7DTO>();
            foreach (var place in places )
            {
                var images = await  _context.PlaceImages.Where(p => p.PlaceId == place.Id).Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").FirstOrDefaultAsync();               
                place.PictureUrl = images;
            }
            for(int i = 0; i < 47; i++)
            {
                var test = new B7B7DTO();
                test.PlaceId = places[i].Id;
                test.CategoryName =await  _context.Categories.Where(c=>c.Id == places[i].CategoryId).Select(c=>c.CategoryName).FirstOrDefaultAsync();
                test.Name = places[i].Name;
                test.CulturalTips = places[i].CultureTips;
                test.Address =await  _context.Locations.Where(l=>l.Id == places[i].locationId).Select(l=>l.Address).FirstOrDefaultAsync();
                test.Duration = places[i].Duration;
                test.CloseTime = places[i].CloseTime;
                test.OpenTime = places[i].OpenTime;
                test.Description = places[i].Description;
                test.imageURLs = places[i].PictureUrl;
                test.LocationLink = await _context.Locations.Where(l => l.Id == places[i].locationId).Select(l => l.LocationLink).FirstOrDefaultAsync();
                test.TouristAdult = await _context.Prices.Where(p=>p.Id == places[i].priceId).Select(p=>p.TouristAdult).FirstOrDefaultAsync();
                test.EgyptianAdult = await _context.Prices.Where(p => p.Id == places[i].priceId).Select(p => p.EgyptianAdult).FirstOrDefaultAsync();
                B7B7places.Add(test);
            }
            return Ok(B7B7places);
        }
    }
}
