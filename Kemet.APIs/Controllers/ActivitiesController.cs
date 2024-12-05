using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.PlaceSpecs;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Kemet.APIs.Controllers
{
   
    public class ActivitiesController : BaseApiController
    {
        private readonly IGenericRepository<Activity> _activityRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        public ActivitiesController(
            IGenericRepository<Activity>ActivityRepo
            ,IMapper mapper
            ,AppDbContext context)
        {
            _activityRepo = ActivityRepo;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet()] // /api/Activities  Get
        public async Task<ActionResult<IEnumerable<ActivityDTOs>>> GetActivity()
        {
            try
            {
                var spec = new ActivityWithPlacesSpecifications();
                var activity = (IEnumerable<Activity>)await _activityRepo.GetAllWithSpecAsync(spec);
                if(activity == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
                var Activities = _mapper.Map<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>(activity);
                if(Activities == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
                var Result = Activities.Where(A => A.imageURLs.Any()).ToList();
                return Ok(Result);
            }catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
                        
                        
                        
        }

        [HttpGet("GetActivityByID")]
        public async Task<ActionResult<ActivityDTOs>> GetActivityById(int ActivityID)
        {

            try
            {
                var spec = new ActivityWithPlacesSpecifications(ActivityID);
                var activity =await _activityRepo.GetWithSpecAsync(spec);
                if (activity == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
             
                var Activities = _mapper.Map<Activity, ActivityDTOs>(activity);
                if (Activities == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
                var fetchedReviews =  await   _context.Reviews.Where(r=>r.ActivityId == ActivityID).ToListAsync();
                Activities.Reviews = fetchedReviews;
                var Result = Activities;
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
        

    }
}                       
                        