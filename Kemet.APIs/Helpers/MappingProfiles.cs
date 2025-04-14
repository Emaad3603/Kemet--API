using AutoMapper;
using AutoMapper.Execution;
using Kemet.APIs.DTOs;
using Kemet.APIs.DTOs.DetailedDTOs;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.ReviewsDTOs;
using Kemet.APIs.DTOs.WishlistDtos;
using Kemet.Core.Entities;
using Kemet.Core.Entities.AI_Entites;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.Images;
using Kemet.Core.Entities.WishlistEntites;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Options;

namespace Kemet.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        

        public MappingProfiles()
        {
          

            CreateMap<Place, PlacesDto>()
              .ForMember(d=>d.PlaceID,o=>o.MapFrom(s=>s.Id))
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
                  PlaceID=a.Id,
                  Name = a.Name,
                  ImageURLs = a.Images.Select(img => $"{"https://localhost:7051"}{img.ImageUrl}").ToList(),
                  AverageRating = a.AverageRating,
                  RatingsCount = a.RatingsCount ,
                  
                  
                
              }).ToList());
            //==============================================================
            /*  CreateMap<AddPlaceDtos, Place>()
         .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
         .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
         .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
             src.ImageURLs.Select(url => new PlaceImage { ImageUrl = url }).ToList()))
         .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
             new Price
             {
                 EgyptianAdult = src.EgyptianAdultCost,
                 EgyptianStudent = src.EgyptianStudentCost,
                 TouristAdult = src.TouristAdultCost,
                 TouristStudent = src.TouristStudentCost

             }))
         .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
             new Category
             {
                 CategoryName = src.CategoryName,
                 CategoryType = "Historical"
             }));  */
            CreateMap<AddPlaceDtos, Place>()
      .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
      .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
      .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
          new Category
          {
              CategoryName = src.CategoryName,
              CategoryType = "place"
          }));

            //==============================================================
            CreateMap<AddActivityDto, Activity>()
     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))  // Map Name
     .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))  // Map Duration
     .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))  // Map Description
     .ForMember(dest => dest.CulturalTips, opt => opt.MapFrom(src => src.CultureTips))  // Map CultureTips
     .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl))  // Map PictureUrl
     .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
         new Category
         {
             CategoryName = src.CategoryName,
             CategoryType = "Activity"  // Use a suitable category type
         }))
     .ForMember(dest => dest.Price, opt => opt.MapFrom(src => new Price
     {
         EgyptianAdult = src.EgyptianAdultCost,
         EgyptianStudent = src.EgyptianStudentCost,
         TouristAdult = src.TouristAdultCost,
         TouristStudent = src.TouristStudentCost
     }));
     //.ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
     //    src.ImageURLs.Select(url => new ActivityImage { ImageUrl = url }).ToList()));  // Map ImageURLs to ActivityImage collection

            CreateMap<Activity, ActivityDTOs>()
                           .ForMember(d=>d.ActivityId , o=>o.MapFrom(s=>s.Id))
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
                    ActivityId=a.Id,
                    Name = a.Name,
                    Duration = a.Duration,

                   
                    AverageRating= a.AverageRating ,
                    RatingsCount = a.RatingsCount ,
                }).ToList());


            CreateMap<TravelAgencyPlan, TravelAgencyPlanDTOs>()
                .ForMember(d=>d.PlanId ,o=>o.MapFrom(s=>s.Id))
                  .ForMember(d => d.Name, o => o.MapFrom(s => s.TravelAgency.UserName))
                  .ForMember(d => d.PlanName, o => o.MapFrom(s => s.PlanName))
                  .ForMember(d => d.Duration, o => o.MapFrom(s => s.Duration))
                  .ForMember(d => d.PictureUrl, o => o.MapFrom<TravelAgencyPlanUrlResolver>())
                  .ForMember(d => d.EgyptianAdult, o => o.MapFrom(s => s.Price.EgyptianAdult))
                  .ForMember(d => d.EgyptianStudent, o => o.MapFrom(s => s.Price.EgyptianStudent))
                  .ForMember(d => d.TouristAdult, o => o.MapFrom(s => s.Price.TouristAdult))
                  .ForMember(d => d.TouristStudent, o => o.MapFrom(s => s.Price.TouristStudent))
                  .ForMember(d=>d.PlanLocation , o=>o.MapFrom(s=>s.PlanLocation))
                  .ForMember(d => d.AverageRating, o => o.MapFrom(s => s.AverageRating))
                  .ForMember(d => d.RatingsCount, o => o.MapFrom(s => s.RatingsCount))
                  .ForMember(d => d.Reviews, o => o.MapFrom(s => s.Reviews))
                  .ReverseMap();

            CreateMap<IEnumerable<TravelAgencyPlan>, IEnumerable<TravelAgencyPlanDTOs>>()
              .ConvertUsing((src, dest) => src.Select(a => new TravelAgencyPlanDTOs
              {   PlanId = a.Id ,
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
                 .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
                 .ForMember(dest=>dest.UserName , opt=>opt.MapFrom(src=>src.USERNAME))
                 .ForMember(dest=>dest.UserImageUrl , opt=>opt.MapFrom(src=>src.UserImageURl))     
                 .ForMember(dest=>dest.Date,opt=>opt.MapFrom(src=>src.Date))
                 .ForMember(dest => dest.VisitorType, opt => opt.MapFrom(src => src.VisitorType))
                 .ForMember(dest => dest.ReviewTitle, opt => opt.MapFrom(src => src.ReviewTitle))


                 .ReverseMap();

            CreateMap<Wishlist, WishlistDto>()
                .ForMember(w => w.Places, o => o.MapFrom(s => s.Places))
                .ForMember(w => w.Activities, o => o.MapFrom(s => s.Activities))
                .ForMember(w => w.Plans, o => o.MapFrom(s => s.Plans))
                .ReverseMap();


            CreateMap<Activity, DetailedActivityDTOs>()
                .ForMember(a=>a.ActivityId ,o=>o.MapFrom(s=>s.Id))
                 .ForMember(w => w.Name, o => o.MapFrom(s => s.Name))
                 .ForMember(w => w.Duration, o => o.MapFrom(o => o.Duration))
                 .ForMember(w => w.CulturalTips, o => o.MapFrom(o => o.CulturalTips))
                 .ForMember(w => w.Description, o => o.MapFrom(o => o.Description))
                 .ForMember(w => w.GroupSize, o => o.MapFrom(o => o.GroupSize))
                 .ForMember(w => w.OpenTime, o => o.MapFrom(o => o.OpenTime))
                 .ForMember(w => w.CloseTime, o => o.MapFrom(o => o.CloseTime))
                 .ForMember(w => w.RatingsCount, o => o.MapFrom(o => o.RatingsCount))
                 .ForMember(w => w.AverageRating, o => o.MapFrom(o => o.AverageRating))
                 .ForMember(w=>w.Reviews,o=>o.MapFrom(o=>o.Reviews))
                 .ForMember(d => d.imageURLs, o => o.MapFrom<DetailedActivityPicturesUrlResolver>())
                 .ForMember(d => d.EgyptianAdult, o => o.MapFrom(s => s.Price.EgyptianAdult))
                 .ForMember(d => d.EgyptianStudent, o => o.MapFrom(s => s.Price.EgyptianStudent))
                 .ForMember(d => d.TouristAdult, o => o.MapFrom(s => s.Price.TouristAdult))
                 .ForMember(d => d.TouristStudent, o => o.MapFrom(s => s.Price.TouristStudent))
                 .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
                 .ReverseMap();
            CreateMap<Place,DetailedPlaceDto>()
                .ForMember(w=>w.PlaceId,o=>o.MapFrom(s=>s.Id))
                 .ForMember(w => w.Name, o => o.MapFrom(s => s.Name))
                 .ForMember(w => w.Duration, o => o.MapFrom(o => o.Duration))
                 .ForMember(w => w.CulturalTips, o => o.MapFrom(o => o.CultureTips))
                 .ForMember(w => w.Description, o => o.MapFrom(o => o.Description))
                 .ForMember(w => w.OpenTime, o => o.MapFrom(o => o.OpenTime))
                 .ForMember(w => w.CloseTime, o => o.MapFrom(o => o.CloseTime))
                 .ForMember(w => w.RatingsCount, o => o.MapFrom(o => o.RatingsCount))
                 .ForMember(w => w.AverageRating, o => o.MapFrom(o => o.AverageRating))
                 .ForMember(w => w.Reviews, o => o.MapFrom(o => o.Reviews))
                 .ForMember(d => d.imageURLs, o => o.MapFrom<DetailedPlacePictureUrlResolver>())
                 .ForMember(d => d.EgyptianAdult, o => o.MapFrom(s => s.Price.EgyptianAdult))
                 .ForMember(d => d.EgyptianStudent, o => o.MapFrom(s => s.Price.EgyptianStudent))
                 .ForMember(d => d.TouristAdult, o => o.MapFrom(s => s.Price.TouristAdult))
                 .ForMember(d => d.TouristStudent, o => o.MapFrom(s => s.Price.TouristStudent))
                 .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category.CategoryName))
                 .ReverseMap();

            CreateMap<TravelAgencyPlan,DetailedTravelAgencyPlanDto>()
                .ForMember(Tp=>Tp.PlanId , o=>o.MapFrom(s=>s.Id))
                .ForMember(Tp=>Tp.PlanName,o=>o.MapFrom(s=>s.PlanName))
                .ForMember(Tp=>Tp.Description,o=>o.MapFrom(s=>s.Description))
                .ForMember(Tp => Tp.Duration, o => o.MapFrom(s => s.Duration))
                 .ForMember(w => w.RatingsCount, o => o.MapFrom(o => o.RatingsCount))
                 .ForMember(w => w.AverageRating, o => o.MapFrom(o => o.AverageRating))
                 .ForMember(w => w.Reviews, o => o.MapFrom(o => o.Reviews))
                 .ForMember(d => d.imageURLs, o => o.MapFrom<DetailedPlansPicturesUrlResolver>())
                 .ForMember(d => d.EgyptianAdult, o => o.MapFrom(s => s.Price.EgyptianAdult))
                 .ForMember(d => d.EgyptianStudent, o => o.MapFrom(s => s.Price.EgyptianStudent))
                 .ForMember(d => d.TouristAdult, o => o.MapFrom(s => s.Price.TouristAdult))
                 .ForMember(d => d.TouristStudent, o => o.MapFrom(s => s.Price.TouristStudent))
                 .ForMember(d=>d.planLocation ,o=>o.MapFrom(o => o.PlanLocation))
                 .ReverseMap();
           
        }

    }
}
