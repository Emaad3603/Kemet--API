using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications;
using Kemet.Core.Specifications.PlaceSpecs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kemet.APIs.Controllers
{
  
    public class PlacesController : BaseApiController
    {
        private readonly IGenericRepository<Place> _placesRepo;
        private readonly IMapper _mapper;

        //getAll
        //getByid
        public PlacesController(IGenericRepository<Place>placesRepo,IMapper mapper)
        {
            _placesRepo = placesRepo;
            _mapper = mapper;
        }




        [HttpGet] // /api/places  Get
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces()
        {
            try { 

            var spec = new PlaceWithCategoriesAndactivitiesSpecifications();
            var places = await _placesRepo.GetAllWithSpecAsync(spec);

                if(places == null)
                {
                return NotFound(new ApiResponse(404, "No Places found "));
                }

            var Result=  _mapper.Map<IEnumerable<Place>, IEnumerable<PlacesDto>>(places);

                 if (Result == null)
                 {
                return NotFound(new ApiResponse(404, "No Places found "));
                 }

                   return Ok(Result);
        }catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }

}
       

    }
}
