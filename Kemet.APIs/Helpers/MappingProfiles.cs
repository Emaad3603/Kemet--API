using AutoMapper;
using AutoMapper.Execution;
using Kemet.APIs.DTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.WishlistDtos;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.WishlistEntites;

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

              .ForMember(d => d.AverageRating, o => o.MapFrom(s => s.AverageRating))
              .ForMember(d => d.RatingsCount, o => o.MapFrom(s => s.RatingsCount))
              .ForMember(d => d.Reviews, o => o.MapFrom(s => s.Reviews))
              .ReverseMap();

            CreateMap<IEnumerable<Place>, IEnumerable<PlacesDto>>()
              .ConvertUsing((src, dest) => src.Select(a => new PlacesDto
              {
                  Name = a.Name,
                  ImageURLs = a.Images.Select(img => $"{"https://localhost:7051"}{img.ImageUrl}").ToList(),
                  AverageRating = a.AverageRating,
                  RatingsCount = a.RatingsCount ,
                  
                  
                
              }).ToList());

            CreateMap<Activity, ActivityDTOs>()
     .ForMember(d => d.Name, o => o.MapFrom(s => s.Name))
     .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
     .ForMember(d => d.imageURLs, o => o.MapFrom<ActivityPicturesUrlResolver>())
     .ForMember(d => d.AverageRating, o => o.MapFrom(s => s.AverageRating))
     .ForMember(d => d.RatingsCount, o => o.MapFrom(s => s.RatingsCount))
     .ForMember(d => d.Reviews, o => o.MapFrom(s => s.Reviews.Select(r => new ReviewDto
     {
         Comment = r.Comment,
         Rating = r.Rating
     }).ToList()))
     .ReverseMap();




            CreateMap<IEnumerable<Activity>, IEnumerable<ActivityDTOs>>()
                .ConvertUsing((src, dest) => src.Select(a => new ActivityDTOs
                {
                    Name = a.Name,
                    Duration = a.Duration,

                    imageURLs = a.Images.Select(img => $"{"https://localhost:7051"}{img.ImageUrl}"/*img => img.ImageUrl*/).ToList(),
                    AverageRating= a.AverageRating ,
                    RatingsCount = a.RatingsCount ,
                }).ToList());


            CreateMap<TravelAgencyPlan, TravelAgencyPlanDTOs>()
                  .ForMember(d => d.Name, o => o.MapFrom(s => s.TravelAgency.UserName))
                  .ForMember(d => d.PlanName, o => o.MapFrom(s => s.PlanName))
                  .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                  .ForMember(d => d.PictureUrl, o => o.MapFrom<TravelAgencyPlanUrlResolver>())
                  .ForMember(d => d.EgyptianAdult, o => o.MapFrom(s => s.Price.EgyptianAdult))
                  .ForMember(d => d.EgyptianStudent, o => o.MapFrom(s => s.Price.EgyptianStudent))
                  .ForMember(d => d.TouristAdult, o => o.MapFrom(s => s.Price.TouristAdult))
                  .ForMember(d => d.TouristStudent, o => o.MapFrom(s => s.Price.TouristStudent))

                  .ForMember(d => d.AverageRating, o => o.MapFrom(s => s.AverageRating))
                  .ForMember(d => d.RatingsCount, o => o.MapFrom(s => s.RatingsCount))
                  .ForMember(d => d.Reviews, o => o.MapFrom(s => s.Reviews))
                  .ReverseMap();

            CreateMap<IEnumerable<TravelAgencyPlan>, IEnumerable<TravelAgencyPlanDTOs>>()
              .ConvertUsing((src, dest) => src.Select(a => new TravelAgencyPlanDTOs
              {
                  Name = a.PlanName, 
                  AverageRating = a.AverageRating ,
                  RatingsCount = a.RatingsCount ,
              
                  
              }).ToList());

            CreateMap<Review, ReviewDto>()
               
                 .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                 .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                 .ForMember(dest => dest.ActivityId, opt => opt.MapFrom(src => src.ActivityId))
                 .ForMember(dest => dest.PlaceId, opt => opt.MapFrom(src => src.PlaceId))
                 .ForMember(dest => dest.TravelAgencyPlanId, opt => opt.MapFrom(src => src.TravelAgencyPlanId))
                 .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl));
                  .ReverseMap();

            CreateMap<Wishlist, WishlistDto>()
                .ForMember(w => w.Places, o => o.MapFrom(s => s.Places))
                .ForMember(w => w.Activities, o => o.MapFrom(s => s.Activities))
                .ForMember(w => w.Plans, o => o.MapFrom(s => s.Plans))
                .ReverseMap();




        }
    }
}
