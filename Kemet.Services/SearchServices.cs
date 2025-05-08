using Kemet.Core.Entities.Identity;
using Kemet.Core.Entities.ModelView;
using Kemet.Core.Services.Interfaces;
using Kemet.Repository.Data;
using Microsoft.AspNetCore.Identity;
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

        public async Task<RagionalSearchDTO> SearchAll(string textA , UserManager<AppUser> userManager)
        {
            try
            {//Where(A => EF.Functions.Like(A.Name, textA)
                var text = textA.ToLower();
                var places = await _context.Places.AsNoTracking().Where(P => P.Name.Contains(text) || P.Location.Address.Contains(text) || P.CultureTips.Contains(text) || P.Description.Contains(text) || P.Category.CategoryName.Contains(text)).ToListAsync();
                var Activities = await _context.Activities.AsNoTracking().Where(A => A.Name.Contains(textA)|| A.Location.Address.Contains(text) || A.Category.CategoryName.Contains(text)).ToListAsync();
                var travelAgencyPlans = await _context.TravelAgencyPlans.AsNoTracking().Where(A => A.PlanName.Contains(text) || A.Description.Contains(text) || A.TravelAgency.UserName.Contains(text)).ToListAsync();
                var travelAgencies = (await userManager.GetUsersInRoleAsync("TravelAgency"))
                                                       .OfType<TravelAgency>()
                                                       .Where(t => t.UserName.Contains(text))
                                                       .Select(t => new TravelAgencySearchDTO
                                                       {
                                                           UserName = t.UserName,
                                                           pictureUrl = t.ImageURL,  // Ensure the property name matches your model
                                                           Description = t.Description,
                                                           Bio = t.Bio
                                                       })
                                                       .ToList();
                var result = new RagionalSearchDTO()
                {
                    Places = places,
                    Activities = Activities,
                    TravelAgencyPlans = travelAgencyPlans ,
                    TravelAgencies = travelAgencies
                    
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
