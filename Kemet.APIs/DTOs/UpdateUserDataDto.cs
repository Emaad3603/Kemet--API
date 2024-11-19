namespace Kemet.APIs.DTOs
{
    public class UpdateUserDataDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public List<int> InterestCategoryIds { get; set; } = new List<int>();
    }
}
