using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.PlaceSpecs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Controllers
{
   
    public class ActivitiesController : BaseApiController
    {
        private readonly IGenericRepository<Activity> _activityRepo;
        private readonly IMapper _mapper;

        public ActivitiesController(IGenericRepository<Activity>ActivityRepo,IMapper mapper)
        {
            _activityRepo = ActivityRepo;
            _mapper = mapper;
        }

        [HttpGet] // /api/places  Get
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivity()
        {
            var spec = new ActivityWithPlacesSpecifications();
            var activity = await _activityRepo.GetAllWithSpecAsync(spec);
            var Result = _mapper.Map<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>(activity);


            return Ok(Result);


        }
    }
}
