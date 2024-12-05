using Kemet.Core.Entities;
using Kemet.Core.Entities.Images;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class ActivityDTOs
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }

        public List<string> imageURLs { get; set; }
        
    }
}
