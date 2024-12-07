namespace Kemet.APIs.DTOs.ProfileDTOs
{
    public class UpdateUserDataDto
    {
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public List<int> InterestCategoryIds { get; set; } = new List<int>();

   //     public IFormFile? ProfileImage { get; set; }
    //    public IFormFile? BackgroundImage { get; set; }

        public string? Bio { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? WebsiteLink { get; set; }
    }
}
