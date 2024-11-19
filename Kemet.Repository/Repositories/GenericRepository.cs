using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Core.Specifications;
using Kemet.Repository.Data;
using Kemet.Repository.Specification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Kemet.Repository.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            //if (typeof(T) == typeof(Place)) 
            //{
            //    //Include(P => P.Category).Include(P=>P.Activities).Include(P=>P.Location)
            //    return (IReadOnlyList<T>)await _context.Places.ToListAsync();
            
            //}  
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
           
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<int> GetCountAsync(ISpecifictions<T> spec )
        {
            return await ApplySpecifications(spec).CountAsync();
        }


        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifictions<T> spec)
        {
           return await  SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec).ToListAsync();

        }

        public async Task<T?> GetWithSpecAsync(ISpecifictions<T> spec)
        {
            return await SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec).FirstOrDefaultAsync();
        }

        private IQueryable<T> ApplySpecifications (ISpecifictions<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);
        }


        public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);


        public void Delete(T entity) => _context.Set<T>().Remove(entity);
       

        public void Update(T entity)=> _context.Set<T>().Update(entity);
       
    }
}

