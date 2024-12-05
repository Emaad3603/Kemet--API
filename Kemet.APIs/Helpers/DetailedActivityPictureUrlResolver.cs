using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class DetailedActivityPicturesUrlResolver : IValueResolver<Activity, DetailedActivityDTOs, List<string>>
    {
        private readonly IConfiguration _configuration;
        public DetailedActivityPicturesUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<string> Resolve(Activity source, DetailedActivityDTOs destination, List<string> destMember, ResolutionContext context)
        {
            return source.Images.Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToList();
        }
    }
}
