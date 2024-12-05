namespace Kemet.APIs.DTOs.ReviewsDTOs
{
    public class ReviewDto
    {
        public string? UserId { get; set; }

        public string? UserName { get; set; }

        public string? UserImageUrl {  get; set; }

        public string? ReviewImageUrl { get; set; }


        public string Comment { get; set; }
        public int Rating { get; set; }

        public IFormFile? Image { get; set; }
        public int? ActivityId { get; set; }
        public int? PlaceId { get; set; }
        public int? TravelAgencyPlanId { get; set; }
    }
}
