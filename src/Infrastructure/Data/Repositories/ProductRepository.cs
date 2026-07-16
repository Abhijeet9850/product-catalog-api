using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Product?> GetWithItemsAsync(int id, CancellationToken ct = default)
            => await DbSet.Include(p => p.Items)
                          .AsNoTracking()
                          .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public class ItemRepository : GenericRepository<Item>, IItemRepository
    {
        public ItemRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}