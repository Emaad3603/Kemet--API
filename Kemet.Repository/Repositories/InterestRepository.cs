using Kemet.Core.Entities.Intersts;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications;
using Kemet.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Repository.Repositories
{
    public class InterestRepository :GenericRepository<CustomerInterest>, IinterestsRepository
    {
       

        public InterestRepository(AppDbContext context):base(context)
        {
            
        }

       

        public async Task AddInterestsAsync(string userId, List<int> categoriesId)
        {
            foreach (var category in categoriesId)
            {

                var data = new CustomerInterest()
                {
                    CustomerId = userId,
                    CategoryId = category
                };
                await _context.CustomerInterests.AddAsync(data);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInterestsAsync(string userId)
        {
           var userInterests = await _context.CustomerInterests.Where(CI => CI.CustomerId == userId).ToListAsync();
           _context.RemoveRange(userInterests);
           await _context.SaveChangesAsync();
            
        }

        public async Task<List<int>> GetUserInterestsAsync(string userId)
        {
            var userInterests =    await _context.CustomerInterests.Where(CI=>CI.CustomerId == userId).ToListAsync();
            var categoryIds = new List<int>();
            foreach (var category in userInterests)
            {
                categoryIds.Add(category.CategoryId);
            }
            return categoryIds;
        }

        public async Task UpdateInterestsAsync(string userId, List<int> categoriesId)
        {
           await  DeleteInterestsAsync(userId);
           await AddInterestsAsync(userId, categoriesId);

        }
    }
}
