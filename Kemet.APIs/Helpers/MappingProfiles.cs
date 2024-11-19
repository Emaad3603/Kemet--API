using AutoMapper;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Place, HomePlacesDto>()
              .ForMember(d=>d.Name,o=>o.MapFrom(s=>s.Name))
              .ForMember(d=>d.Description,o=>o.MapFrom(s=>s.Description)).ReverseMap();

            CreateMap<Activity, ActivityDTOs>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration)).ReverseMap(); 


            CreateMap<TravelAgencyPlan, TravelAgencyPlanDTOs>()
                  .ForMember(d => d.Name, o => o.MapFrom(s => s.TravelAgency.UserName))
                  .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration)).ReverseMap(); 
            
        }
    }
}
