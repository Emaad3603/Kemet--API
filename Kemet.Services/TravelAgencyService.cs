using Kemet.Core.Entities;
using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.Images;
using Kemet.Core.Entities.ModelView;
using Kemet.Core.Services.Interfaces;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Services
{
    public class TravelAgencyService : ITravelAgencyService
    {
        private readonly AppDbContext _context;
        

        public TravelAgencyService(AppDbContext context)
        {
            _context = context;
           
        }

        public async Task<ICollection<TravelAgencyBookedCustomersModelView>> GetCustomersAsync(string TravelAgencyName)
        {
            //var booking = await _context.BookedTrips
            //                            .Where(b => b.TravelAgencyName == TravelAgencyName)
            //                            .GroupBy(b => b.CustomerID)
            //                            .Select(g => g.OrderByDescending(b => b.CreatedAt).FirstOrDefault()) // Or use FirstOrDefault() or Max() depending on preference
            //                            .Select(b => new TravelAgencyBookedCustomersModelView()
            //                            {
            //                                PlanName = b.travelAgencyPlan.PlanName,
            //                                PlanID = b.travelAgencyPlan.Id,
            //                                BookedCategory = b.BookedCategory,
            //                                BookingId = b.Id,
            //                                CustomerCountry = b.Customer.Country,
            //                                CustomerEmail = b.Customer.Email,
            //                                CustomerName = b.Customer.UserName,
            //                                CustomerNumber = b.Customer.PhoneNumber,
            //                                Date = b.ReserveDate,
            //                                CreatedAt = b.CreatedAt
            //                            })
            //                            .ToListAsync();
                         var bookings = await _context.BookedTrips
                 .Where(b => b.TravelAgencyName == TravelAgencyName)
                 .Include(b => b.Customer)
                 .Include(b => b.travelAgencyPlan)
                 .ToListAsync();

            var latestBookingsPerCustomer = bookings
                .GroupBy(b => b.CustomerID)
                .Select(g => g.OrderByDescending(b => b.CreatedAt).FirstOrDefault())
                .Where(b => b != null)
                .Select(b => new TravelAgencyBookedCustomersModelView
                {
                    PlanName = b.travelAgencyPlan.PlanName,
                    PlanID = b.travelAgencyPlan.Id,
                    BookedCategory = b.BookedCategory,
                    BookingId = b.Id,
                    CustomerCountry = b.Customer.Country,
                    CustomerEmail = b.Customer.Email,
                    CustomerName = b.Customer.UserName,
                    CustomerNumber = b.Customer.PhoneNumber,
                    Date = b.ReserveDate,
                    CreatedAt = b.CreatedAt
                })
                .ToList();


            return latestBookingsPerCustomer;
        }

        public async  Task<ICollection<TravelAgencyPlan>> GetTravelAgencyPlans(string travelAgencyID , IConfiguration configuration)
        {
           var plans =  await _context.TravelAgencyPlans.Where(T => T.TravelAgencyId == travelAgencyID).Include(t=>t.images).Include(t=>t.Reviews).ToListAsync();
           foreach (var plan in plans)
            {
                plan.TravelAgency = null;
                plan.PictureUrl = $"{configuration["BaseUrl"]}/{plan.PictureUrl}";
                foreach(var image in plan.images) {
                    {
                        image.ImageURl = $"{configuration["BaseUrl"]}/{image.ImageURl}";
                    } }
                foreach (var review in plan.Reviews)
                {
                    review.ImageUrl = $"{configuration["BaseUrl"]}/{review.ImageUrl}";
                }
                plan.TravelAgency = null;
            }
            return plans;
        }

        public async Task<ICollection<Review>> GetTravelAgencyReviews(string travelAgencyID, IConfiguration configuration)
        {
            var reviews = await _context.Reviews.Where(T => T.TravelAgencyID == travelAgencyID).ToListAsync();
            foreach (var review in reviews)
            {   
                review.ImageUrl = $"{configuration["BaseUrl"]}/{review.ImageUrl}";
            }
            return reviews;
           
        }

        public async Task<(double satisfactionRate, Dictionary<int, int> ratingCounts)> CalculateSatisfactionRateAsync(string TravelAgencyId)
        {
            var ratingGroups = await _context.Reviews
                .Where(r=>r.TravelAgencyID==TravelAgencyId)
                .GroupBy(r => r.Rating)
                .Select(g => new
                {
                    Rating = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            int totalReviews = ratingGroups.Sum(g => g.Count);
            int maxScore = totalReviews * 5;

            if (totalReviews == 0)
                return (0, new Dictionary<int, int>());

            int weightedScore = ratingGroups.Sum(g => g.Rating * g.Count);
            double satisfactionRate = (double)weightedScore / maxScore * 100;

            // Dictionary to hold each rating and its count (1 to 5)
            var ratingCounts = Enumerable.Range(1, 5)
                .ToDictionary(
                    rating => rating,
                    rating => ratingGroups.FirstOrDefault(g => g.Rating == rating)?.Count ?? 0
                );

            return (Math.Round(satisfactionRate, 2), ratingCounts);
        }

        public async Task<Dictionary<string, decimal>> GetMonthlyRevenueAsync(string agencyName)
        {
            var bookings = await _context.BookedTrips
                .Include(b => b.travelAgencyPlan)
                    .ThenInclude(p => p.Price)
                .Where(b => b.TravelAgencyName == agencyName)
                .ToListAsync();

            var monthlyRevenue = bookings
                .GroupBy(b => b.CreatedAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(b => GetPriceByReserveType(b) * b.NumOfPeople)
                })
                .ToList();

            // Fill all months with 0 if not present
            var result = Enumerable.Range(1, 12)
                .ToDictionary(
                    m => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m),
                    m => monthlyRevenue.FirstOrDefault(x => x.Month == m)?.Revenue ?? 0
                );

            return result;
        }

        public async Task<Dictionary<string, int>> GetTopBookedPlansAsync(string agencyName)
        {
            var planCounts = await _context.BookedTrips
                .Where(b => b.TravelAgencyName == agencyName)
                .GroupBy(b => b.travelAgencyPlan.PlanName)
                .Select(g => new
                {
                    PlanName = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            var top3 = planCounts.Take(3).ToDictionary(x => x.PlanName, x => x.Count);
            int othersCount = planCounts.Skip(3).Sum(x => x.Count);

            if (othersCount > 0)
                top3["Others"] = othersCount;

            return top3;
        }


        private decimal GetPriceByReserveType(BookedTrips b)
        {
            var price = b.travelAgencyPlan?.Price;

            return b.ReserveType switch
            {
                "EgyptianAdult" => price?.EgyptianAdult ?? 0,
                "EgyptianStudent" => price?.EgyptianStudent ?? 0,
                "TouristAdult" => price?.TouristAdult ?? 0,
                "TouristStudent" => price?.TouristStudent ?? 0,
                _ => 0
            };
        }

        public async Task<(bool IsSuccess, string Message, TravelAgencyPlan? Plan)> AddPlanAsync(CreatePlanDto dto)
        {
            try
            {
               var list = await  _context.Prices.ToListAsync();
                string _plansUploadFolder = Path.Combine("wwwroot", "uploads", "Plans");
                var price = new Price
                {   Id = list.Count()+1,
                    EgyptianAdult = dto.Price.EgyptianAdult,
                    EgyptianStudent = dto.Price.EgyptianStudent,
                    TouristAdult = dto.Price.TouristAdult,
                    TouristStudent = dto.Price.TouristStudent
                };

                await _context.Prices.AddAsync(price);
                await _context.SaveChangesAsync();

                // Ensure folder exists
                if (!Directory.Exists(_plansUploadFolder))
                    Directory.CreateDirectory(_plansUploadFolder);

                // Save main picture
                string mainImageUrl = null;
                if (dto.PictureUrl != null)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dto.PictureUrl.FileName);
                    string filePath = Path.Combine(_plansUploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.PictureUrl.CopyToAsync(stream);
                    }

                    mainImageUrl = "/uploads/Plans/" + uniqueFileName;
                }

                var plan = new TravelAgencyPlan
                {
                    PlanName = dto.PlanName,
                    Duration = dto.Duration,
                    Description = dto.Description,
                    PlanAvailability = dto.PlanAvailability,
                    PictureUrl = mainImageUrl,
                    TravelAgencyId = dto.TravelAgencyId,
                    PlanLocation = dto.PlanLocation,
                    priceId = price.Id,
                    Price = price,
                    HalfBoardPriceAddittion = dto.HalfBoardPriceAddittion,
                    FullBoardPriceAddition  = dto.FullBoardPriceAddition,
                };

                // Save gallery images
                foreach (var file in dto.Images)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(_plansUploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    string relativeUrl = "/uploads/Plans/" + uniqueFileName;

                    plan.images.Add(new TravelAgencyPlanImages
                    {
                        ImageURl = relativeUrl
                    });
                }

                await _context.TravelAgencyPlans.AddAsync(plan);
                await _context.SaveChangesAsync();

                return (true, "Plan created successfully.", plan);
            }
            catch (Exception ex)
            {
                return (false, "Something went wrong: " + ex.Message, null);
            }
        }

        public async Task<(bool IsSuccess, string Message)> UpdatePlanAsync(UpdatePlanDto dto)
        {
            string _plansUploadFolder = Path.Combine("wwwroot", "uploads", "Plans");
            var plan = await _context.TravelAgencyPlans
                .Include(p => p.images)
                .Include(p => p.Price)
                .FirstOrDefaultAsync(p => p.Id == dto.Id);

            if (plan == null)
                return (false, "Plan not found.");

            // Update simple fields
            plan.PlanName = dto.PlanName;
            plan.Duration = dto.Duration;
            plan.Description = dto.Description;
            plan.PlanAvailability = dto.PlanAvailability;
            plan.PlanLocation = dto.PlanLocation;
            plan.HalfBoardPriceAddittion = dto.HalfBoardPriceAddittion;
            plan.FullBoardPriceAddition = dto.FullBoardPriceAddition;

            // Update price
            if (plan.Price != null)
            {
                plan.Price.EgyptianAdult = dto.Price.EgyptianAdult;
                plan.Price.EgyptianStudent = dto.Price.EgyptianStudent;
                plan.Price.TouristAdult = dto.Price.TouristAdult;
                plan.Price.TouristStudent = dto.Price.TouristStudent;
            }

            // Handle main picture update
            if (dto.PictureUrl != null)
            {
                if (!Directory.Exists(_plansUploadFolder))
                    Directory.CreateDirectory(_plansUploadFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(dto.PictureUrl.FileName);
                string filePath = Path.Combine(_plansUploadFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.PictureUrl.CopyToAsync(stream);
                }

                plan.PictureUrl = "/uploads/Plans/" + uniqueFileName;
            }

            // Replace images if new ones are uploaded
            if (dto.NewImages != null && dto.NewImages.Any())
            {
                // Optional: delete old image files from disk

                _context.TravelAgencyPlanImages.RemoveRange(plan.images);

                foreach (var file in dto.NewImages)
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    string filePath = Path.Combine(_plansUploadFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    plan.images.Add(new TravelAgencyPlanImages
                    {
                        ImageURl = "/uploads/Plans/" + uniqueFileName
                    });
                }
            }

            _context.TravelAgencyPlans.Update(plan);
            await _context.SaveChangesAsync();

            return (true, "Plan updated successfully.");
        }



    }
}
