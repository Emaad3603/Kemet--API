namespace Kemet.APIs.DTOs
{
    public class UploadProfileImageDto
    {
        public IFormFile? ProfileImage { get; set; }
        public IFormFile? BackgroundImage { get; set; }
    }
}
