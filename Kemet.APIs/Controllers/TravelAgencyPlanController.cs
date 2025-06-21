using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.APIs.Helpers;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.TravelAgencyPlanSpecs;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Crypto;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kemet.APIs.Controllers
{
  
    public class TravelAgencyPlanController : BaseApiController
    {
        private readonly IGenericRepository<TravelAgencyPlan> _travelagencyplanRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICacheRepository _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);
        public TravelAgencyPlanController(
            IGenericRepository<TravelAgencyPlan>TravelagencyplanRepo
            ,IMapper mapper
            ,AppDbContext context
            , IConfiguration configuration 
            , UserManager<AppUser> userManager
            ,ICacheRepository cache)
        {
            _travelagencyplanRepo = TravelagencyplanRepo;
            _mapper = mapper;
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _cache = cache;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TravelAgencyPlan>>> GetTravelAgencyPlan()
        {
            string cacheKey = "Plans_list";
            var cached = await _cache.GetAsync(cacheKey);
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                ReferenceHandler = ReferenceHandler.Preserve
            };

            jsonOptions.Converters.Add(new DateOnlyConverter());

            if (!string.IsNullOrEmpty(cached))
            {
                try
                {
                    // Deserialize directly as IEnumerable since CacheRepository uses ReferenceHandler.Preserve
                    var result = JsonSerializer.Deserialize<IEnumerable<DetailedTravelAgencyPlanDto>>(cached, jsonOptions);
                    if (result != null)
                    {
                        return Ok(result);
                    }
                }
                catch (JsonException)
                {
                    // If deserialization fails, continue to fetch fresh data
                }
            }

            var spec = new TravelAgencyPlanSpecifications();
            var travelAgencyPlan = await _travelagencyplanRepo.GetAllWithSpecAsync(spec);
            var Result = _mapper.Map<IEnumerable<TravelAgencyPlan>, IEnumerable<DetailedTravelAgencyPlanDto>>(travelAgencyPlan);
           
            // CacheRepository will handle the serialization with ReferenceHandler.Preserve
            await _cache.SetAsync(cacheKey, Result, _cacheDuration);
            
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
                plans.MoreImagesUrl = images;
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
