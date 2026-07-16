using Application.Interfaces;
using Infrastructure.Data.Repositories;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IProductRepository? _products;
        private IItemRepository? _items;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _products ??= new ProductRepository(_context);

        public IItemRepository Items => _items ??= new ItemRepository(_context);

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _context.SaveChangesAsync(ct);


        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}