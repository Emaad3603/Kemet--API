using Kemet.Core.Entities;

namespace Kemet.APIs.DTOs
{
    public class UserBookingHistoryDto
    {
        public int bookingId {  get; set; }
        public int TravelAgencyPlanID { get; set; }
        public TravelAgencyPlan TravelAgencyPlan { get; set; }
        public string TravelAgencyName { get; set; }
        public int NumOfPeople { get; set; }
        public string ReserveType { get; set; }
        public DateOnly ReserveDate { get; set; }
        public string BookedCategory { get; set; }
        public decimal BookedPrice { get; set; }
        public decimal FullBookedPrice { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string StripePaymentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PlanName { get; set; }
        public int PlanDuration { get; set; }
        public string PlanImage { get; set; }
    }
}
