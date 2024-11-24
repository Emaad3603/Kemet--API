using Kemet.Core.Entities;
using Kemet.Core.Entities.Images;

namespace Kemet.APIs.DTOs.HomePageDTOs
{
    public class ActivityDTOs
    {
        public string Name { get; set; }
        public int Duration { get; set; }

        public List<ActivityImage> images { get; set; }
        
    }
}
