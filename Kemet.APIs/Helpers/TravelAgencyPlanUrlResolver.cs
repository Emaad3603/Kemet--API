using AutoMapper;
using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class TravelAgencyPlanUrlResolver : IValueResolver<TravelAgencyPlan, TravelAgencyPlanDTOs, string>
    {
        private readonly IConfiguration _configuration;

        public TravelAgencyPlanUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(TravelAgencyPlan source, TravelAgencyPlanDTOs destination, string destMember, ResolutionContext context)
        {
          
            if (!string.IsNullOrEmpty(source.PictureUrl)) 
            {

                return $"{_configuration["BaseUrl"]}/{source.PictureUrl}";
            }
            return string.Empty;
        }
    }
}
