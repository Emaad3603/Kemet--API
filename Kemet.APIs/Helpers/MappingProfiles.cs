using AutoMapper;
using AutoMapper.Execution;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Place, PlacesDto>()
              .ForMember(d=>d.Name,o=>o.MapFrom(s=>s.Name))
              .ForMember(d=>d.Description,o=>o.MapFrom(s=>s.Description))
              .ReverseMap();

            CreateMap<Activity, ActivityDTOs>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration)).ReverseMap(); 


            CreateMap<TravelAgencyPlan, TravelAgencyPlanDTOs>()
                  .ForMember(d => d.Name, o => o.MapFrom(s => s.TravelAgency.UserName))
                  .ForMember(d=>d.PlanName,o=>o.MapFrom(s=>s.PlanName))
                  .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                  .ForMember(d=>d.PictureUrl, o=>o.MapFrom<TravelAgencyPlanUrlResolver>())
                  .ForMember(d=>d.EgyptianAdult,o=>o.MapFrom(s=>s.Price.EgyptianAdult))
                  .ForMember(d => d.EgyptianStudent, o => o.MapFrom(s => s.Price.EgyptianStudent))
                  .ForMember(d => d.TouristAdult, o => o.MapFrom(s => s.Price.TouristAdult))
                  .ForMember(d => d.TouristStudent, o => o.MapFrom(s => s.Price.TouristStudent))
                  .ReverseMap(); 

            
        }
    }
}
