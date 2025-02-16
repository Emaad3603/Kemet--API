using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        
        public string? ImageURL { get; set; }

        public string? BackgroundImageURL { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
        public Point? Location { get; set; } // Stores the user's location as a spatial point

    }
}
