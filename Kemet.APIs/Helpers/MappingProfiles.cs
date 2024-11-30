﻿using AutoMapper;
using AutoMapper.Execution;
using Kemet.APIs.DTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;

namespace Kemet.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
       

      
        public MappingProfiles()
        {
            CreateMap<Place, PlacesDto>()
              .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
              .ForMember(d => d.Description, o => o.MapFrom(s => s.Description))
              .ForMember(d => d.ImageURLs, o => o.MapFrom<placesPicturesUrlResolver>())
              .ReverseMap();

            CreateMap<Activity, ActivityDTOs>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
                .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                 .ForMember(d => d.imageURLs, o => o.MapFrom<ActivityPicturesUrlResolver>())
                .ReverseMap();

            // Map IEnumerable<Activity> to IEnumerable<ActivityDTOs>

            CreateMap<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>()
                .ConvertUsing((src, dest) => src.Select(a => new ActivityDTOs
                {
                    Name = a.Name,
                    Duration = a.Duration,
                    imageURLs = a.Images.Select(img => $"{"https://localhost:7051"}{img.ImageUrl}"/*img => img.ImageUrl*/).ToList()
                }).ToList());


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
