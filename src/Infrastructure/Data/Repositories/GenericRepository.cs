using System.Linq.Expressions;
using Application.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext Context;
        protected readonly DbSet<T> DbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
            => await DbSet.FindAsync(new object[] { id }, ct);

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
            => await DbSet.AsNoTracking().ToListAsync(ct);

        public IQueryable<T> Query(bool asNoTracking = true)
            => asNoTracking ? DbSet.AsNoTracking() : DbSet;

        public async Task AddAsync(T entity, CancellationToken ct = default)
            => await DbSet.AddAsync(entity, ct);

        public void Update(T entity) => DbSet.Update(entity);

        public void Remove(T entity) => DbSet.Remove(entity);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await DbSet.AnyAsync(predicate, ct);
    }
}