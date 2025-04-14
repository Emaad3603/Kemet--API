using Kemet.Core.Entities;
using System;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<(bool success, string message, string clientSecret)> CreatePaymentIntentAsync(BookedTrips booking, string currency = "usd");
        Task<(bool success, string message)> ConfirmPaymentAsync(string paymentIntentId);
        Task<(bool success, string message)> HandleStripeWebhookAsync(string json, string stripeSignature);
    }
} 