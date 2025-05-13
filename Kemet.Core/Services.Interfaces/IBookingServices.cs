using Kemet.Core.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Services.Interfaces
{
    public interface IBookingServices
    {
        Task<ICollection<BookedTrips>> getUserBookedtripsAsync(string UserID);
        Task<BookedTrips> getBookedTrip(int bookingID);
    }
}
