using Kemet.Core.Entities.AI_Entites;
using Microsoft.AspNetCore.Http;

namespace Kemet.Core.Entities.ModelView
{
    public class CreatePlanDto
    {
        public string PlanName { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public string? PlanAvailability { get; set; }
        public IFormFile PictureUrl { get; set; } // this can also be an IFormFile if uploaded
        public string? TravelAgencyId { get; set; }
        public string? PlanLocation { get; set; }
        public decimal? HalfBoardPriceAddittion { get; set; }

        public decimal? FullBoardPriceAddition { get; set; }
        public PriceDto Price { get; set; }

        public List<IFormFile> Images { get; set; } = new();
    }
}
