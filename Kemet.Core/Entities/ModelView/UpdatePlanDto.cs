using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.ModelView
{
    public class UpdatePlanDto
    {
        public int Id { get; set; }

        public string PlanName { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public string? PlanAvailability { get; set; }
        public IFormFile? PictureUrl { get; set; } // Optional
        public string? TravelAgencyId { get; set; }
        public string? PlanLocation { get; set; }

        public int? PriceId { get; set; }
        public PriceDto? Price { get; set; }

        public List<IFormFile>? NewImages { get; set; } = new();
    }
}
