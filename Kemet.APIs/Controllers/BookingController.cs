using Kemet.APIs.DTOs.BookingDTOs;
using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Kemet.APIs.Controllers
{

    public class BookingController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IGenericRepository<BookedTrips> _bookingRepository;
        private readonly IGenericRepository<TravelAgencyPlan> _travelAgencyPlanRepository;
        private readonly IBookingServices _bookingServices;

        public BookingController
            (UserManager<AppUser> userManager,
              IGenericRepository<BookedTrips> bookingRepository,
              IGenericRepository<TravelAgencyPlan> travelAgencyPlanRepository,
              IBookingServices bookingServices

            )
        {
            _userManager = userManager;
            _bookingRepository = bookingRepository;
            _travelAgencyPlanRepository = travelAgencyPlanRepository;
            _bookingServices = bookingServices;
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
            var trip = new BookedTrips()
            {
                TrabelAgencyPlanID = book.TravelAgencyPlanID,
                travelAgencyPlan = plan,
                CustomerID = user.Id,
                BookedPrice = book.BookedPrice,
                NumOfPeople = book.NumOfPeople,
                ReserveDate = book.ReserveDate,
                ReserveType = book.ReserveType,
                TravelAgencyName = BookedAgency.UserName

            };
            await _bookingRepository.AddAsync(trip);

            trip.Customer = null;
            trip.travelAgencyPlan.TravelAgency = null;
            
            return Ok(trip);
        }

        [HttpGet("GetUserBookedTrips")]
        public async Task<ActionResult<ICollection<BookedTrips>>> GetUserBookedTrips()
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
            return Ok(trips);
        }
    }
}
