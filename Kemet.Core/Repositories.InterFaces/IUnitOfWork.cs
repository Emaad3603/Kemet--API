using Kemet.Core.Entities;
using Kemet.Core.RepositoriesInterFaces;
using System;
using System.Threading.Tasks;

namespace Kemet.Core.Repositories.InterFaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        IReviewRepository ReviewRepository { get; }
        IWishlistRepository WishlistRepository { get; }
        IinterestsRepository InterestsRepository { get; }
        Task<int> Complete();
    }
}