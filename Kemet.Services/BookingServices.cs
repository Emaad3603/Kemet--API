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
          var plans = await   _context.BookedTrips.Where(b=>b.CustomerID == UserID).Include(b=>b.travelAgencyPlan).ToListAsync() ;
          return plans;
        }
    }
}
