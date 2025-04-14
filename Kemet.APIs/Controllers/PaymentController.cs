using Kemet.Core.Entities;
using Kemet.Core.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using Stripe;
using Microsoft.EntityFrameworkCore;
using Kemet.Repository.Data;
using Kemet.Core.RepositoriesInterFaces;

namespace Kemet.APIs.Controllers
{
    [Authorize]
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IGenericRepository<BookedTrips> _bookingRepository;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public PaymentController(
            IPaymentService paymentService, 
            IGenericRepository<BookedTrips> bookingRepository,
            IConfiguration configuration,
            AppDbContext context)
        {
            _paymentService = paymentService;
            _bookingRepository = bookingRepository;
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("create-payment-intent/{bookingId}")]
        public async Task<IActionResult> CreatePaymentIntent(int bookingId)
        {
            var booking = await _bookingRepository.GetAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found");

            // Verify the booking belongs to the current user
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (booking.Customer.Email != userEmail)
                return Unauthorized("This booking does not belong to you");

            var (success, message, clientSecret) = await _paymentService.CreatePaymentIntentAsync(booking);
            
            if (!success)
                return BadRequest(message);

            return Ok(new { clientSecret });
        }

        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromBody] string paymentIntentId)
        {
            var (success, message) = await _paymentService.ConfirmPaymentAsync(paymentIntentId);
            
            if (!success)
                return BadRequest(message);

            return Ok(new { message });
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            
            try
            {
                var stripeSignature = Request.Headers["Stripe-Signature"];
                var (success, message) = await _paymentService.HandleStripeWebhookAsync(json, stripeSignature);
                
                if (!success)
                    return BadRequest(message);

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("history/{bookingId}")]
        public async Task<IActionResult> GetPaymentHistory(int bookingId)
        {
            var booking = await _bookingRepository.GetAsync(bookingId);
            if (booking == null)
                return NotFound("Booking not found");

            // Verify the booking belongs to the current user
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (booking.Customer.Email != userEmail)
                return Unauthorized("This booking does not belong to you");

            var paymentHistory = await _context.PaymentHistories
                .Where(ph => ph.BookedTripsId == bookingId)
                .OrderByDescending(ph => ph.EventDate)
                .ToListAsync();

            return Ok(paymentHistory);
        }

        [HttpGet("user-history")]
        public async Task<IActionResult> GetUserPaymentHistory()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized();

            var paymentHistory = await _context.PaymentHistories
                .Include(ph => ph.BookedTrips)
                    .ThenInclude(bt => bt.travelAgencyPlan)
                .Where(ph => ph.BookedTrips.Customer.Email == userEmail)
                .OrderByDescending(ph => ph.EventDate)
                .ToListAsync();

            return Ok(paymentHistory);
        }
    }
} 