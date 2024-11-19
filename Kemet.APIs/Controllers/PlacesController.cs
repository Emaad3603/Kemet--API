using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
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
            var spec = new PlaceWithCategoriesAndactivitiesSpecifications();
            var places = await _placesRepo.GetAllWithSpecAsync(spec);
            var Result=  _mapper.Map<IEnumerable<Place>, IEnumerable<HomePlacesDto>>(places);


           return Ok(Result);


        }
       

    }
}
