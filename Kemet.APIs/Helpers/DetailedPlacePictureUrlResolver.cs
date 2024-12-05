using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class DetailedPlacePictureUrlResolver : IValueResolver<Place, DetailedPlaceDto, List<string>>
    {
        private readonly IConfiguration _configuration;
        public DetailedPlacePictureUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<string> Resolve(Place source, DetailedPlaceDto destination, List<string> destMember, ResolutionContext context)
        {
            return source.Images.Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToList();
        }
    }
}
