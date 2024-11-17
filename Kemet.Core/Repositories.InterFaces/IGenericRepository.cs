using Kemet.Core.Entities;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kemet.Core.Specifications;

namespace Kemet.Core.RepositoriesInterFaces
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        //GetAll 
        //GetByID



        Task<T?> GetAsync(int id);

        Task<IReadOnlyList<T>> GetAllAsync();


        Task<T?> GetWithSpecAsync(ISpecifictions<T> spec);

        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifictions<T> spec);

        Task<int> GetCountAsync(ISpecifictions<T> spec);

        Task AddAsync(T entity);

        void Delete(T entity);

        void Update(T entity);
    }
}
