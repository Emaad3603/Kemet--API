using Kemet.APIs.DTOs;
using Kemet.APIs.DTOs.BookingDTOs;
using Kemet.APIs.Errors;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{

    public class BookingController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<BookedTrips> _bookingRepository;
        private readonly IGenericRepository<TravelAgencyPlan> _travelAgencyPlanRepository;
        private readonly IBookingServices _bookingServices;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public BookingController
            (UserManager<AppUser> userManager,
              IGenericRepository<BookedTrips> bookingRepository,
              IGenericRepository<TravelAgencyPlan> travelAgencyPlanRepository,
              IBookingServices bookingServices ,
              AppDbContext context,
              IConfiguration configuration

            )
        {
            _userManager = userManager;
            _bookingRepository = bookingRepository;
            _travelAgencyPlanRepository = travelAgencyPlanRepository;
            _bookingServices = bookingServices;
            _context = context;
            _configuration = configuration;
        }
        [HttpPost("BookTrip")]
        public async Task<IActionResult> BookTrip(BookDTO book)
        {
            // Check if the user is signed in
            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Fetch the user's location from the database
            var user = new Customer();
            if (userEmail != null)
            {
                 user = await _userManager.FindByEmailAsync(userEmail) as Customer;
            }
            if (user == null) return NotFound("User not found.");

            var BookedAgency = await _userManager.FindByIdAsync(book.TravelAgencyID) as TravelAgency;

            if (BookedAgency == null) return NotFound("Travel Agency not found.");
            var plan = await _travelAgencyPlanRepository.GetAsync(book.TravelAgencyPlanID);
            if (plan == null) return NotFound("Plan not found");
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (book.ReserveDate < today) { return BadRequest("this date isn't correct"); }

            var ORiginalPlan = await _context.TravelAgencyPlans.Where(p => p.Id == book.TravelAgencyPlanID).Include(p=>p.Price).FirstOrDefaultAsync();
            if (ORiginalPlan?.Price == null) return BadRequest("Price information not found for the plan");

            string PRiceCategory;

            var priceMapping = new Dictionary<decimal, string>
                                {
                                    { (decimal)(ORiginalPlan.Price.EgyptianStudent ?? 0), "EgyptianStudent" },
                                    { (decimal)(ORiginalPlan.Price.EgyptianAdult ?? 0), "EgyptianAdult" },
                                    { (decimal)(ORiginalPlan.Price.TouristAdult ?? 0), "TouristAdult" },
                                    { (decimal)(ORiginalPlan.Price.TouristStudent ?? 0), "TouristStudent" }
                                };

            PRiceCategory = priceMapping.TryGetValue(book.BookedPrice, out var category) ? category : "Unknown";
            var trip = new BookedTrips()
            {
                TrabelAgencyPlanID = book.TravelAgencyPlanID,
                travelAgencyPlan = plan,
                CustomerID = user.Id,
                BookedCategory = PRiceCategory,
                NumOfPeople = book.NumOfPeople,
                ReserveDate = book.ReserveDate,
                ReserveType = book.ReserveType,
                TravelAgencyName = BookedAgency.UserName,
                BookedPrice = book.BookedPrice,
                FullBookedPrice =book.FullBookedPrice
            };
            await _bookingRepository.AddAsync(trip);
            
            return Ok(new { 
                success = true,
                message = "Booking created successfully",
                bookingId = trip.Id ,
                numOfPeople = book.NumOfPeople,
                reserveType = book.ReserveType,
                reserveDate = book.ReserveDate,
                visitorType = PRiceCategory.ToString(),
            });
        }

        [HttpGet("GetUserBookedTrips")]
        public async Task<ActionResult<ICollection<UserBookingHistoryDto>>> GetUserBookedTrips()
        {
            try
            {
                // Check if the user is signed in
                string userEmail = User.FindFirstValue(ClaimTypes.Email);

                // Fetch the user's location from the database
                var user = new Customer();
                if (userEmail != null)
                {
                     user = await _userManager.FindByEmailAsync(userEmail) as Customer;
                }
                if (user == null) return NotFound("User not found.");
                var trips =  await  _bookingServices.getUserBookedtripsAsync(user.Id);
                if (trips == null) return NotFound();
                var resTrips = new List<UserBookingHistoryDto>();
                foreach (var trip in trips)
                {
                    var resTrip = new UserBookingHistoryDto()
                    {
                        bookingId = trip.Id,
                        TravelAgencyPlanID = trip.TrabelAgencyPlanID,
                        TravelAgencyPlan = trip.travelAgencyPlan,
                        TravelAgencyName = trip.TravelAgencyName,

                        NumOfPeople = trip.NumOfPeople,
                        ReserveType = trip.ReserveType,
                        ReserveDate = trip.ReserveDate,
                        BookedCategory = trip.BookedCategory,
                        BookedPrice = trip.BookedPrice,
                        FullBookedPrice = trip.FullBookedPrice,
                        PaymentStatus = trip.PaymentStatus,
                        PaymentDate = trip.PaymentDate,
                        StripePaymentId = trip.StripePaymentId,
                        CreatedAt = trip.CreatedAt,
                        PlanName = trip.travelAgencyPlan.PlanName,
                        PlanDuration = trip.travelAgencyPlan.Duration,
                        PlanImage =$"{_configuration["BaseUrl"]}/{trip.travelAgencyPlan.PictureUrl}"
                    };
                    resTrips.Add(resTrip);
                }
                return Ok(resTrips);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, $"Internal server error: {ex.Message}"));
            }
        }
        [HttpGet("GetBookedTrip")]
        public async Task<ActionResult<BookedTrips>> GetBookedTripByID (int id)
        {
           var res = await _bookingServices.getBookedTrip(id);
           res.Customer = null;
           return Ok(res);
        }
    }
}
