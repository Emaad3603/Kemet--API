using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Entities.AI_Entites
{
    public class AiResponseDto
    {
        public bool Success { get; set; }
        public TravelPlan TravelPlan { get; set; }
    }

    public class TravelPlan
    {
        public Itinerary Itinerary { get; set; }
        public string TotalCost { get; set; }
    }

    public class Itinerary
    {
        public List<DayPlan> Days { get; set; }
        public string TotalBudget { get; set; }
        public int TotalDays { get; set; }
    }

    public class DayPlan
    {
        public Dictionary<string, ActivityPlace> Day { get; set; }
    }

    public class ActivityPlace
    {
        public ActivityDto Activity { get; set; }
        public PlaceDto Place { get; set; }
    }

    public class ActivityDto
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string EntryCost { get; set; }
        public string ImageURL { get; set; }
    }

    public class PlaceDto
    {
        public int PlaceId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string EntryCost { get; set; }
        public string ImageURLs { get; set; }
    }
}
