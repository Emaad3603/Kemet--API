using AutoMapper;
using Kemet.APIs.DTOs;
using Kemet.Core.Entities.Identity;

namespace Kemet.APIs.Helpers
{
    public class UserDataUrlResolver : IValueResolver<Customer, GetCurrentUserDataResponseDto, string>
    {
        private readonly IConfiguration _configuration;

        public UserDataUrlResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Customer source, GetCurrentUserDataResponseDto destination, string destMember, ResolutionContext context)
        {
            // Resolve ProfileImage URL
            if (!string.IsNullOrEmpty(source.ImageURL))
            {
                return $"{_configuration["BaseUrl"]}/{source.ImageURL}";
            }

            // Resolve BackgroundImage URL
            if (!string.IsNullOrEmpty(source.BackgroundImageURL))
            {
                return $"{_configuration["BaseUrl"]}/{source.BackgroundImageURL}";
            }

            return string.Empty;
        }
    }
}
