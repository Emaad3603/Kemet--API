using AutoMapper;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class DetailedPlansPicturesUrlResolver : IValueResolver<TravelAgencyPlan, DetailedTravelAgencyPlanDto, string>
    {
        private readonly IConfiguration _configuration;

        public DetailedPlansPicturesUrlResolver(IConfiguration configuration)
        {
           _configuration = configuration;
        }
        public string Resolve(TravelAgencyPlan source, DetailedTravelAgencyPlanDto destination, string destMember, ResolutionContext context)
        {
            return source.PictureUrl = $"{_configuration["BaseUrl"]}/{source.PictureUrl}";
        }
    }
}
