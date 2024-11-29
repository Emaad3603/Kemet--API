using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class placesPicturesUrlResolver : IValueResolver<Place, PlacesDto, List<string>>
    {
        private readonly IConfiguration _configuration;

        public placesPicturesUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<string> Resolve(Place source, PlacesDto destination, List<string> destMember, ResolutionContext context)
        {
            //if (!string.IsNullOrEmpty(source.PictureUrl))
            //{

                //  return $"{_configuration["BaseUrl"]}/{source.PictureUrl}";
                return source.Images.Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToList();
           // }
           // return null;
        }
    }
}
