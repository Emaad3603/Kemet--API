using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Kemet.APIs.DTOs.HomePageDTOs;
using Kemet.APIs.DTOs.ReviewsDTOs;
using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.IdentityDTOs
{
    public class TravelAgencyProfileDto
    {
        public string TravelAgencyId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? FacebookURL { get; set; }
        public string? InstagramURL { get; set; }
        public string? Bio { get; set; }
        public string ProfileURl { get; set; }
        public string BackgroundURL { get; set; }
        
        public double AverageRating { get; set; }

        public ICollection<TravelAgencyPlan?> Plan { get; set; } 

        public ICollection<Review?> reviews { get; set; }
    }
}
