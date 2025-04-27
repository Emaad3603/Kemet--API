using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.TravelAgencyPlanSpecs;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Numerics;

namespace Kemet.APIs.Controllers
{
  
    public class TravelAgencyPlanController : BaseApiController
    {
        private readonly IGenericRepository<TravelAgencyPlan> _travelagencyplanRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public TravelAgencyPlanController(
            IGenericRepository<TravelAgencyPlan>TravelagencyplanRepo
            ,IMapper mapper
            ,AppDbContext context
            , IConfiguration configuration 
            , UserManager<AppUser> userManager)
        {
            _travelagencyplanRepo = TravelagencyplanRepo;
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }
        [HttpGet] // /api/places  Get
        public async Task<ActionResult<IEnumerable<TravelAgencyPlan>>>GetTravelAgencyPlan ()
        {
            var spec = new TravelAgencyPlanSpecifications();
            var travelAgencyPlan = await _travelagencyplanRepo.GetAllWithSpecAsync(spec);
            var Result = _mapper.Map<IEnumerable<TravelAgencyPlan>, IEnumerable<DetailedTravelAgencyPlanDto>>(travelAgencyPlan);

            return Ok(Result);


        }

        [HttpGet("GetTravelAgencyPlanByID")]
        public async Task<ActionResult<TravelAgencyPlanDTOs>> GetTravelAgencyPlanById(int PlanId)
        {

            try
            {
                var spec = new TravelAgencyPlanSpecifications(PlanId);
                var plan = await _travelagencyplanRepo.GetWithSpecAsync(spec);
                if (plan == null)
                {
                    return NotFound(new ApiResponse(404, "No Exclusive Packages found "));
                }

                var plans = _mapper.Map<TravelAgencyPlan, DetailedTravelAgencyPlanDto>(plan);
                if (plans == null)
                {
                    return NotFound(new ApiResponse(404, "No Exclusive Packages found "));
                }
                var fetchedReviews = await _context.Reviews.Where(r => r.TravelAgencyPlanId == PlanId).ToListAsync();
                foreach (var fetchedReview in fetchedReviews)
                {
                    fetchedReview.TravelAgencyPlan = null;
                    fetchedReview.ImageUrl = $"{_configuration["BaseUrl"]}{fetchedReview.ImageUrl}";
                }
                var images =  await  _context.TravelAgencyPlanImages.Where(ti=>ti.TravelAgencyPlanID == PlanId).Select(t => $"{_configuration["BaseUrl"]}{t.ImageURl}").ToListAsync();

                plans.Reviews = fetchedReviews;
                var travelAgency = await _userManager.FindByIdAsync(plan.TravelAgencyId) as TravelAgency;
                plans.TravelAgencyName = travelAgency.UserName;
                plans.TravelAgencyAddress = travelAgency.Address;
                plans.TravelAgencyDescription = travelAgency.Description;
                var Result = plans;
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
