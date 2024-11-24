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
            var place = await _placesRepo.GetAllWithSpecAsync(spec);

                if(place == null)
                {
                return NotFound(new ApiResponse(404, "No Places found "));
                }

            var places=  _mapper.Map<IEnumerable<Place>, IEnumerable<PlacesDto>>(place);

                 if (places == null)
                 {
                return NotFound(new ApiResponse(404, "No Places found "));
                 }
                 var Result = places.Where(P=>P.ImageURLs.Any()).ToList();

                   return Ok(Result);
        }catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }

}
       

    }
}
