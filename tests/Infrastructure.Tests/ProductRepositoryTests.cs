using Domain.Entities;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Infrastructure.Tests
{
    public class ProductRepositoryTests
    {
        private static ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ThenSave_PersistsProduct()
        {
            await using var context = CreateContext();
            var repo = new ProductRepository(context);

            var product = new Product { ProductName = "Widget", CreatedBy = "abhijeet", CreatedOn = DateTime.UtcNow };
            await repo.AddAsync(product);
            await context.SaveChangesAsync();

            var stored = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Widget");
            stored.Should().NotBeNull();
        }

        [Fact]
        public async Task GetWithItemsAsync_ReturnsProductWithRelatedItems()
        {
            await using var context = CreateContext();

            var product = new Product { ProductName = "Widget", CreatedBy = "abhijeet", CreatedOn = DateTime.UtcNow };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            context.Items.Add(new Item { ProductId = product.Id, Quantity = 10 });
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);
            var result = await repo.GetWithItemsAsync(product.Id);

            result.Should().NotBeNull();
            result!.Items.Should().HaveCount(1);
            result.Items.First().Quantity.Should().Be(10);
        }

        [Fact]
        public async Task ExistsAsync_UnknownId_ReturnsFalse()
        {
            await using var context = CreateContext();
            var repo = new ProductRepository(context);

            var exists = await repo.ExistsAsync(p => p.Id == 12345);

            exists.Should().BeFalse();
        }
    }
}
