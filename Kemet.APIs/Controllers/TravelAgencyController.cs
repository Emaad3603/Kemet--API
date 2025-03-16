using Kemet.APIs.DTOs.IdentityDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Controllers
{
    public class TravelAgencyController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITravelAgencyService _travelAgencyService;
        private readonly IConfiguration _configuration;

        public TravelAgencyController(
            UserManager<AppUser> userManager ,
            ITravelAgencyService travelAgencyService ,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _travelAgencyService = travelAgencyService;
            _configuration = configuration;
        }
        [HttpGet]
        public async Task<TravelAgencyProfileDto> GetTravelAgnecy (string travelAgencyName)
        {
            var travelAgency = await _userManager.FindByNameAsync(travelAgencyName) as TravelAgency;
            var plans =  await _travelAgencyService.GetTravelAgencyPlans(travelAgency.Id , _configuration);
            var reviews = await _travelAgencyService.GetTravelAgencyReviews(travelAgency.Id , _configuration);
            var result = new TravelAgencyProfileDto()
            {
                UserName = travelAgency.UserName,
                Address = travelAgency.Address,
                Email = travelAgency.Email,
                FacebookURL = travelAgency.FacebookURL,
                InstagramURL = travelAgency.InstagramURL,
                BackgroundURL = travelAgency.BackgroundImageURL,
                ProfileURl = travelAgency.ImageURL,
                Bio = travelAgency.Bio,
                Description = travelAgency.Description,
                PhoneNumber = travelAgency.PhoneNumber,
                Plan = plans,
                reviews = reviews
            };
            if (result == null)  NotFound(new ApiResponse(404, "No Travel Agency found."));
            return result;
        }
    }
}
