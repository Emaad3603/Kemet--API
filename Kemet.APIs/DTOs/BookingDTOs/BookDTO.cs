using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs.BookingDTOs
{
    public class BookDTO
    {
        public string TravelAgencyID { get; set; }
        public int TravelAgencyPlanID { get; set; }

        public int NumOfPeople { get; set; }

        public string ReserveType { get; set; }

        public DateOnly ReserveDate { get; set; }

        public decimal BookedPrice { get; set; }

        public decimal FullBookedPrice { get; set; }
    }
}
