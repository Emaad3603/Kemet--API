using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications.TravelAgencyPlanSpecs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace Kemet.APIs.Controllers
{
  
    public class TravelAgencyPlanController : BaseApiController
    {
        private readonly IGenericRepository<TravelAgencyPlan> _travelagencyplanRepo;
        private readonly IMapper _mapper;

        public TravelAgencyPlanController(IGenericRepository<TravelAgencyPlan>TravelagencyplanRepo,IMapper mapper)
        {
            _travelagencyplanRepo = TravelagencyplanRepo;
            _mapper = mapper;
        }
        [HttpGet] // /api/places  Get
        public async Task<ActionResult<IEnumerable<TravelAgencyPlan>>>GetTravelAgencyPlan ()
        {
           var spec = new TravelAgencyPlanSpecifications();
          var travelAgencyPlan = await _travelagencyplanRepo.GetAllWithSpecAsync(spec);
            var Result = _mapper.Map<IEnumerable<TravelAgencyPlan>, IEnumerable<TravelAgencyPlanDTOs>>(travelAgencyPlan);

            return Ok(Result);


        }
    }
}
