﻿using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications;
using Kemet.Core.Specifications.ActivitySpecs;
using Kemet.Core.Specifications.PlaceSpecs;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Kemet.APIs.Controllers
{
  
    public class PlacesController : BaseApiController
    {
        private readonly IGenericRepository<Place> _placesRepo;
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;

        //getAll
        //getByid
        public PlacesController(
            IGenericRepository<Place>placesRepo
            ,IMapper mapper
            ,AppDbContext context)
        {
            _placesRepo = placesRepo;
            _mapper = mapper;
            _context = context;
        }




        [HttpGet] // /api/places  Get
        public async Task<ActionResult<IEnumerable<Place>>> GetPlaces()
        {
            try { 

            var spec = new PlaceWithCategoriesAndactivitiesSpecifications();
            var place = (IEnumerable<Place>)await _placesRepo.GetAllWithSpecAsync(spec);

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

        [HttpGet("GetPlaceByID")]
        public async Task<ActionResult<PlacesDto>> GetPlaceById(int PlaceID)
        {

            try
            {
                var spec = new PlaceWithCategoriesAndactivitiesSpecifications(PlaceID);
                var place = await _placesRepo.GetWithSpecAsync(spec);
                if (place == null)
                {
                    return NotFound(new ApiResponse(404, "No Places found "));
                }

                var Places = _mapper.Map<Place, PlacesDto>(place);
                if (Places == null)
                {
                    return NotFound(new ApiResponse(404, "No Places found "));
                }
                var fetchedPlaces = await _context.Reviews.Where(p => p.PlaceId == PlaceID).ToListAsync();
                Places.Reviews = fetchedPlaces;
                var Result = Places;
                return Ok(Result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
