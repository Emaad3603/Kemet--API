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
    public class SearchServices : ISearchInterface
    {
        private readonly AppDbContext _context;

        public SearchServices(AppDbContext context)
        {
            _context = context;
        }

        public async Task<RagionalSearchDTO> SearchAll(string textA)
        {
            try
            {
                var text = textA.ToLower();
                var places = await _context.Places.Where(P => P.Name.Contains(text) || P.Location.Address.Contains(text) || P.CultureTips.Contains(text) || P.Description.Contains(text) || P.Category.CategoryName.Contains(text)).ToListAsync();
                var Activities = await _context.Activities.Where(A => A.Name.Contains(text) || A.Location.Address.Contains(text) || A.Category.CategoryName.Contains(text)).ToListAsync();
                var travelAgencyPlans = await _context.TravelAgencyPlans.Where(A => A.PlanName.Contains(text) || A.Description.Contains(text) || A.TravelAgency.UserName.Contains(text)).ToListAsync();

                var result = new RagionalSearchDTO()
                {
                    Places = places,
                    Activities = Activities,
                    TravelAgencyPlans = travelAgencyPlans
                };
                return result;
            }
            catch (Exception ex)
            {
                return new RagionalSearchDTO();
            }

        }
    
    }

}
