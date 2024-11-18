using Kemet.Core.Entities.Intersts;
using Kemet.Core.RepositoriesInterFaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kemet.Core.Repositories.InterFaces
{
    public interface IinterestsRepository : IGenericRepository<CustomerInterest>
    {
        public Task AddInterestsAsync(string userId, List<int> categoriesId);

        public Task UpdateInterestsAsync(string userId, List<int> categoriesId);

        public Task DeleteInterestsAsync(string userId);

        public Task<List<int>> GetUserInterestsAsync(string userId);
    }
}
