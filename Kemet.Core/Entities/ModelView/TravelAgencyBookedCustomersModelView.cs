using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.ModelView
{
    public class TravelAgencyBookedCustomersModelView
    {
        public string CustomerName { get; set; }

        public int PlanID { get; set; }

        public string PlanName { get; set; }
        [Phone]
        public string CustomerNumber { get; set; }

        public string CustomerEmail { get; set; }

        public string? CustomerCountry { get; set; }

        public string BookedCategory { get; set; }

        public int BookingId { get; set; }

        public DateOnly Date { get; set; }

        public DateTime CreatedAt { get; set; } 
    }
}
