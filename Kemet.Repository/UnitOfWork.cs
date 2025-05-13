using System;
using System.Collections;
using System.Threading.Tasks;
using Kemet.Core.Entities;
using Kemet.Core.Repositories.InterFaces;
using Kemet.Core.RepositoriesInterFaces;
using Kemet.Repository.Data;
using Kemet.Repository.Repositories;

namespace Kemet.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private Hashtable? _repositories;
        private IReviewRepository? _reviewRepository;
        private IWishlistRepository? _wishlistRepository;
        private IinterestsRepository? _interestsRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IReviewRepository ReviewRepository => 
            _reviewRepository ??= new ReviewRepository(_context);

        public IWishlistRepository WishlistRepository =>
            _wishlistRepository ??= new WishlistRepository(_context);

        public IinterestsRepository InterestsRepository =>
            _interestsRepository ??= new InterestRepository(_context);

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            _repositories ??= new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(GenericRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IGenericRepository<TEntity>)_repositories[type]!;
        }
    }
}