using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities
{
    public class PaymentHistory : BaseEntity
    {
        public int BookedTripsId { get; set; }
        public BookedTrips BookedTrips { get; set; }
        public string EventType { get; set; } // PaymentIntentSucceeded, PaymentIntentPaymentFailed, ChargeRefunded
        public string Status { get; set; } // Paid, Failed, Refunded
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string? StripePaymentId { get; set; }
        public string? StripeEventId { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime EventDate { get; set; } = DateTime.UtcNow;
        public string Metadata { get; set; } // Additional payment details in JSON format
    }
} 