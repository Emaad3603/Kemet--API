namespace Kemet.APIs.DTOs
{
    public class PaymentHistoryDto
    {
        public string EventType { get; set; } // PaymentIntentSucceeded, PaymentIntentPaymentFailed, ChargeRefunded
        public string Status { get; set; } // Paid, Failed, Refunded
        public decimal Amount { get; set; }
        public string Currency { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PlanName { get; set; } 

        public int? PlanID { get; set; }
        public int BookingID { get; set; }
    }
}
