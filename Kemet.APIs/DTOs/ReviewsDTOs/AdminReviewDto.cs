using Kemet.Core.Entities;
using System;

namespace Kemet.APIs.DTOs.ReviewsDTOs
{
    public class AdminReviewDto
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateOnly Date { get; set; }
        public string ReviewTitle { get; set; }
        public string VisitorType { get; set; }
        public string UserImageURL { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Review target information
        public string ReviewType { get; set; } // "Place", "Activity", "TravelAgencyPlan", or "TravelAgency"
        public string ItemName { get; set; } // Name of the reviewed item
        
        // Foreign keys
        public int? ActivityId { get; set; }
        public int? PlaceId { get; set; }
        public int? TravelAgencyPlanId { get; set; }
        public string TravelAgencyId { get; set; }
    }
} 