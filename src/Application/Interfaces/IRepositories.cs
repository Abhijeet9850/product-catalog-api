using System.Linq.Expressions;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
        IQueryable<T> Query(bool asNoTracking = true);
        Task AddAsync(T entity, CancellationToken ct = default);
        void Update(T entity);
        void Remove(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    }

    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product?> GetWithItemsAsync(int id, CancellationToken ct = default);
    }

    public interface IItemRepository : IGenericRepository<Item>
    {
    }

    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IItemRepository Items { get; }
        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }
}