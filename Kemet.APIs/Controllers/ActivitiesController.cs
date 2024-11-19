﻿using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
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

        [HttpGet] // /api/Activities  Get
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivity()
        {
            try
            {
                var spec = new ActivityWithPlacesSpecifications();
                var activity = await _activityRepo.GetAllWithSpecAsync(spec);
                if(activity == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
                var Result = _mapper.Map<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>(activity);
                if(Result == null)
                {
                    return NotFound(new ApiResponse(404, "No Activities found "));
                }
                return Ok(Result);
            }catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
                        
                        
                        
        }               
    }                   
}                       
                        