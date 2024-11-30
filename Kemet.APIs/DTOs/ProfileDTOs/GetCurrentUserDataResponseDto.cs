namespace Kemet.APIs.DTOs.ProfileDTOs
{
    public class GetCurrentUserDataResponseDto
    {
        public string UserName { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string SSN { get; set; }
        public string Gender { get; set; }
        public string Nationality { get; set; }

        public string? ProfileImageURL { get; set; }

        public string? BackgroundImageURL { get; set; }

        public List<int> InterestCategoryIds { get; set; } = new List<int>();

        public string? Bio { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? WebsiteLink { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
