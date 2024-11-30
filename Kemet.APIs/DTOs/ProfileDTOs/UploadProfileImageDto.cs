namespace Kemet.APIs.DTOs.ProfileDTOs
{
    public class UploadProfileImageDto
    {
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? BackgroundImage { get; set; }
    }
}
