using Kemet.Core.Entities;
using Kemet.Core.Services.Interfaces;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Services
{
    public class BookingServices : IBookingServices
    {
        private readonly AppDbContext _context;

        public BookingServices(AppDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<BookedTrips>> getUserBookedtripsAsync(string UserID)
        {
          var Trips = await   _context.BookedTrips.Where(b=>b.CustomerID == UserID).Include(b=>b.travelAgencyPlan).ToListAsync() ;
          foreach ( var trip in Trips)
            {
                if (trip.travelAgencyPlan == null)
                {
                    var plan = await _context.TravelAgencyPlans.Where(t=>t.Id == trip.TrabelAgencyPlanID).FirstOrDefaultAsync() ;
                    trip.travelAgencyPlan = plan;
                }
                trip.Customer = null;
            }
          return Trips;
        }

        public async Task<BookedTrips> getBookedTrip (int bookingID)
        {
            var result = await  _context.BookedTrips.Where(b=>b.Id == bookingID).Include(b=>b.Customer).FirstOrDefaultAsync();
            
            return result;
        }
    }
}
