using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.ReviewSpecs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Controllers
{
   
    public class HomeController : BaseApiController
    {
        private readonly IGenericRepository<Place> _placeRepo;
        private readonly IGenericRepository<Activity> _activityRepo;
        private readonly IGenericRepository<TravelAgencyPlan> _travelagencyplanRepo;
        private readonly IMapper _mapper;

        public HomeController(IGenericRepository<Place>placeRepo,IGenericRepository<Activity>activityRepo,IGenericRepository<TravelAgencyPlan>travelagencyplanRepo, IMapper mapper)
        {
          _placeRepo = placeRepo;
          _activityRepo = activityRepo;
          _travelagencyplanRepo = travelagencyplanRepo;
           _mapper = mapper;
        }

        [HttpGet("Places")]
        public async Task<IActionResult> GetPlacesWithRatings() 
        {
            var places = await _placeRepo.GetAllWithSpecAsync(new PlaceWithReviewsSpecifications(0));
            var placesWithRatings = places.Select(p => new
            {
                Place = p.Name,
                AverageRating=p.Reviews.Any()?p.Reviews.Average(r=>r.Rating):0


            }).ToList();
            return Ok(placesWithRatings);
        }
        [HttpGet("activities")]
        public async Task<IActionResult> GetActivitiesWithRatings()
        {
            var activities = await _activityRepo.GetAllAsync(/*new ActivityWithReviewsSpecifications()*/); // Replace with actual filtering

            //var activitiesWithRatings = activities.Select(a => new
            //{                
            //    Activity = a.Name, // Assuming 'Name' is a property of Activity
            //    AverageRating = a.Reviews.Any() ? a.Reviews.Average(r => r.Rating) : 0,

            //}).ToList();
         
            var result = _mapper.Map<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>(activities);
            return Ok(result);
        }
        [HttpGet("TravelAgencyPlan")]
        public async Task<IActionResult> GetTravelPlansWithRatings()
        {
            var travelPlans = await _travelagencyplanRepo.GetAllWithSpecAsync(new TravelAgencyPlansWithReviewsSpecifications(0)); // Replace with actual filtering

            var plansWithRatings = travelPlans.Select(t => new
            {
                Plan = t.PlanName, // Assuming 'Name' is a property of TravelAgencyPlan
                AverageRating = t.Reviews.Any() ? t.Reviews.Average(r => r.Rating) : 0
            }).ToList();

            return Ok(plansWithRatings);
        }

    }
}
