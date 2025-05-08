using Kemet.Core.Entities;
using Kemet.Core.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Kemet.Repository.Data;
using System.Text.Json;
using Stripe;


namespace Kemet.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly string _stripeSecretKey;
        private readonly string _webhookSecret;
        private readonly AppDbContext _context;
        private readonly IEmailSettings _emailSettings;

        public PaymentService(
            IConfiguration configuration, 
            AppDbContext context,
            IEmailSettings emailSettings)
        {
            _configuration = configuration;
            _stripeSecretKey = _configuration["Stripe:SecretKey"];
            _webhookSecret = _configuration["Stripe:WebhookSecret"];
            _context = context;
            _emailSettings = emailSettings;
            StripeConfiguration.ApiKey = _stripeSecretKey;
        }

        public async Task<(bool success, string message, string clientSecret , string paymentIntendId)> CreatePaymentIntentAsync(BookedTrips booking, string currency = "usd")
        {
            try
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(booking.BookedPrice * 100), // Convert to cents
                    Currency = currency,
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "bookingId", booking.Id.ToString() },
                        { "customerId", booking.CustomerID },
                        { "travelAgencyId", booking.TravelAgencyName }
                    }
                };

                var service = new PaymentIntentService();
                var paymentIntent = await service.CreateAsync(options);

                // Record payment history
                await RecordPaymentHistory(booking.Id, "PaymentIntentCreated", "Pending", 
                    booking.BookedPrice, currency, paymentIntent.Id, null, null);

                return (true, "Payment intent created successfully", paymentIntent.ClientSecret,paymentIntent.Id);
            }
            catch (StripeException e)
            {
                return (false, e.Message, null,null);
            }
            catch (Exception e)
            {
                return (false, "An error occurred while creating payment intent", null,null);
            }
        }

        public async Task<(bool success, string message)> ConfirmPaymentAsync(string paymentIntentId)
        {
            try
            {
                var service = new PaymentIntentService();
                var paymentIntent = await service.GetAsync(paymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    return (true, "Payment confirmed successfully");
                }

                return (false, "Payment not successful");
            }
            catch (StripeException e)
            {
                return (false, e.Message);
            }
            catch (Exception e)
            {
                return (false, "An error occurred while confirming payment");
            }
        }

        public async Task<(bool success, string message)> HandleStripeWebhookAsync(string json, string stripeSignature)
        {
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _webhookSecret
                );

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        await HandleSuccessfulPayment(paymentIntent, stripeEvent.Id);
                        break;

                    case "payment_intent.payment_failed":
                        var failedPayment = stripeEvent.Data.Object as PaymentIntent;
                        await HandleFailedPayment(failedPayment, stripeEvent.Id);
                        break;

                    case "charge.refunded":
                        var refund = stripeEvent.Data.Object as Charge;
                        await HandleRefund(refund, stripeEvent.Id);
                        break;
                }


                return (true, "Webhook handled successfully");
            }
            catch (StripeException e)
            {
                return (false, e.Message);
            }
            catch (Exception e)
            {
                return (false, "An error occurred while handling webhook");
            }
        }

        private async Task HandleSuccessfulPayment(PaymentIntent paymentIntent, string stripeEventId)
        {
            var bookingId = int.Parse(paymentIntent.Metadata["bookingId"]);
            var booking = await _context.BookedTrips
                .Include(b => b.Customer)
                .Include(b => b.travelAgencyPlan)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                booking.PaymentStatus = "Paid";
                booking.PaymentDate = DateTime.UtcNow;
                booking.StripePaymentId = paymentIntent.Id;
                await _context.SaveChangesAsync();

                // Record payment history
                await RecordPaymentHistory(bookingId, "PaymentIntentSucceeded", "Paid",
                    paymentIntent.Amount / 100m, paymentIntent.Currency, paymentIntent.Id,
                    stripeEventId, JsonSerializer.Serialize(paymentIntent));

                // Send confirmation email
                var email = new Email
                {
                    Recipients = booking.Customer.Email,
                    Subject = "Payment Confirmation – Kemet Travel",
                    Body = $@"
Dear {booking.Customer.UserName},

Thank you for booking your trip with Kemet Travel. We are pleased to confirm that your payment has been successfully processed.

Your booking details are as follows:

Plan: {booking.travelAgencyPlan.PlanName}  
Date: {booking.ReserveDate:dddd, dd MMMM yyyy}  
Number of Guests: {booking.NumOfPeople}  
Total Amount Paid: ${booking.BookedPrice} USD

If you have any questions or need further assistance, please do not hesitate to contact us.

We look forward to providing you with a memorable experience!

Best regards,  
Kemet Travel Team"
                };


                await _emailSettings.SendEmailAsync(email);
            }
        }

        private async Task HandleFailedPayment(PaymentIntent paymentIntent, string stripeEventId)
        {
            var bookingId = int.Parse(paymentIntent.Metadata["bookingId"]);
            var booking = await _context.BookedTrips
                .Include(b => b.Customer)
                .Include(b => b.travelAgencyPlan)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking != null)
            {
                booking.PaymentStatus = "Failed";
                await _context.SaveChangesAsync();

                // Record payment history
                await RecordPaymentHistory(bookingId, "PaymentIntentPaymentFailed", "Failed",
                    paymentIntent.Amount / 100m, paymentIntent.Currency, paymentIntent.Id,
                    stripeEventId, JsonSerializer.Serialize(paymentIntent));

                // Send failure notification email
                var email = new Email
                {
                    Recipients = booking.Customer.Email,
                    Subject = "Payment Failed - Kemet Travel",
                    Body = $@"
Dear {booking.Customer.UserName},

We regret to inform you that your payment for the following booking has failed:

Booking Details:
- Plan: {booking.travelAgencyPlan.PlanName}
- Date: {booking.ReserveDate}
- Number of People: {booking.NumOfPeople}
- Amount: {booking.BookedPrice} USD

Please try the payment again or contact our support team for assistance.

Best regards,
Kemet Travel Team"
                };

                await _emailSettings.SendEmailAsync(email);
            }
        }

        private async Task HandleRefund(Charge charge, string stripeEventId)
        {
            var booking = await _context.BookedTrips
                .Include(b => b.Customer)
                .Include(b => b.travelAgencyPlan)
                .FirstOrDefaultAsync(b => b.StripePaymentId == charge.PaymentIntentId);

            if (booking != null)
            {
                booking.PaymentStatus = "Refunded";
                await _context.SaveChangesAsync();

                // Record payment history
                await RecordPaymentHistory(booking.Id, "ChargeRefunded", "Refunded",
                    charge.Amount / 100m, charge.Currency, charge.PaymentIntentId,
                    stripeEventId, JsonSerializer.Serialize(charge));

                // Send refund notification email
                var email = new Email
                {
                    Recipients = booking.Customer.Email,
                    Subject = "Payment Refunded - Kemet Travel",
                    Body = $@"
Dear {booking.Customer.UserName},

Your payment for the following booking has been refunded:

Booking Details:
- Plan: {booking.travelAgencyPlan.PlanName}
- Date: {booking.ReserveDate}
- Number of People: {booking.NumOfPeople}
- Refunded Amount: {booking.BookedPrice} USD

The refund has been processed and should appear in your account within 5-10 business days.

If you have any questions, please contact our support team.

Best regards,
Kemet Travel Team"
                };

                await _emailSettings.SendEmailAsync(email);
            }
        }

        private async Task RecordPaymentHistory(
            int bookedTripsId,
            string eventType,
            string status,
            decimal amount,
            string currency,
            string stripePaymentId,
            string stripeEventId,
            string metadata)
        {
            var pay = await  _context.PaymentHistories.Where(p => p.StripePaymentId == stripePaymentId).FirstOrDefaultAsync();
            if (pay != null)
            {
                pay.Id = bookedTripsId;
                pay.EventType = eventType;
                pay.Status = status;
                pay.Amount = amount;
                pay.Currency = currency;
                pay.StripePaymentId = stripePaymentId;
                pay.StripeEventId = stripeEventId;
                pay.Metadata = metadata;
                await   _context.SaveChangesAsync();
            }
            if (pay is null)
            {
                var paymentHistory = new PaymentHistory
                {
                    BookedTripsId = bookedTripsId,
                    EventType = eventType,
                    Status = status,
                    Amount = amount,
                    Currency = currency,
                    StripePaymentId = stripePaymentId,
                    StripeEventId = stripeEventId,
                    Metadata = metadata
                };

                await _context.PaymentHistories.AddAsync(paymentHistory);
                await _context.SaveChangesAsync();
            }
        }
    }
} 