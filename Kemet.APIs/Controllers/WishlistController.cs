using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.WishlistDtos;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.WishlistEntites;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;

using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{
    public class WishlistController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IGenericRepository<Wishlist> _wishlistRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public WishlistController(
            UserManager<AppUser> userManager ,
            IWishlistRepository wishlistRepository,
            IGenericRepository<Wishlist>wishlistRepo,
            IMapper mapper ,
            AppDbContext context
            
            )
        {
            _userManager = userManager;
            _wishlistRepository = wishlistRepository;
            _wishlistRepo = wishlistRepo;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Wishlist>> GetUserWishlist()
        {
            #region Fetch USER wishlist 
            try
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

                var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
                if (user == null) return NotFound("User not found.");


                var wishlist = await _wishlistRepository.GetUserWishlist(user.Id);
                //  return wishlist;
                if (wishlist == null)
                {
                    return NotFound(new ApiResponse(404, "No wishlist found "));
                }


                var wishlistPlaces = new List<PlacesDto>();
                var wishlistPlans = new List<TravelAgencyPlanDTOs>();
                var wishlistActivites = new List<ActivityDTOs>();

                foreach (var place in wishlist.Places)
                {
                    var fetchedPlace = await _context.Places.Where(p => p.Id == place.PlaceID).Include(p => p.Images).FirstOrDefaultAsync();
                    var mappedPlaces = _mapper.Map<Place, PlacesDto>(fetchedPlace);
                    wishlistPlaces.Add(mappedPlaces);
                }
                foreach (var plan in wishlist.Plans)
                {
                    var fetchedPlan = await _context.TravelAgencyPlans.Where(p => p.Id == plan.TravelAgencyPlanID).FirstOrDefaultAsync();
                    var mappedPlans = _mapper.Map<TravelAgencyPlan, TravelAgencyPlanDTOs>(fetchedPlan);
                    wishlistPlans.Add(mappedPlans);
                }
                foreach (var activity in wishlist.Activities)
                {
                    var fetchedActivity = await _context.Activities.Where(a => a.Id == activity.Id).Include(a => a.Images).FirstOrDefaultAsync();
                    var mappedActivity = _mapper.Map<Activity, ActivityDTOs>(fetchedActivity);
                    wishlistActivites.Add(mappedActivity);
                }
                var wishlistDto = new WishlistDto()
                {
                    Places = wishlistPlaces,
                    Plans = wishlistPlans,
                    Activities = wishlistActivites
                };

                return Ok(wishlistDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            } 
            #endregion
        }

        [HttpPost("AddActivityToWishlist")]
        public async Task<ActionResult> AddActivityToWishlist(int ActivityID)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            if (user == null) return NotFound("User not found.");

            await  _wishlistRepository.AddActivityToWishlist(user.Id, ActivityID);

            return Ok();
        }

        [HttpPost("AddPlaceToWishlist")]
        public async Task<ActionResult> AddPlaceToWishlist(int PlaceID)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            if (user == null) return NotFound("User not found.");

            await _wishlistRepository.AddPlaceToWishlist(user.Id, PlaceID);

            return Ok();
        }

        [HttpPost("AddPlaneToWishlist")]
        public async Task<ActionResult> AddPlanToWishlist(int PlanID)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            if (user == null) return NotFound("User not found.");

            await _wishlistRepository.AddPlanToWishlist(user.Id, PlanID);

            return Ok();
        }

    }
}
