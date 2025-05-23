﻿using AutoMapper;
using Kemet.APIs.Controllers;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.Helpers
{
    public class ActivityPicturesUrlResolver : IValueResolver<Activity, ActivityDTOs, List<string>>
    {
        private readonly IConfiguration _configuration;

        public ActivityPicturesUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public List<string> Resolve(Activity source, ActivityDTOs destination, List<string> destMember, ResolutionContext context)
        {
            return source.Images.Select(img => $"{_configuration["BaseUrl"]}{img.ImageUrl}").ToList();
        }
    }
}
