using AutoMapper;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct = default)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

            var query = _uow.Products.Query(asNoTracking: true).OrderBy(p => p.Id);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PagedResult<ProductDto>
            {
                Items = _mapper.Map<List<ProductDto>>(items),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var product = await _uow.Products.GetWithItemsAsync(id, ct)
                ?? throw new NotFoundException(nameof(Product), id);

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default)
        {
            var entity = new Product
            {
                ProductName = dto.ProductName,
                CreatedBy = dto.CreatedBy,
                CreatedOn = DateTime.UtcNow
            };

            await _uow.Products.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<ProductDto>(entity);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default)
        {
            var entity = await _uow.Products.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Product), id);

            entity.ProductName = dto.ProductName;
            entity.ModifiedBy = dto.ModifiedBy;
            entity.ModifiedOn = DateTime.UtcNow;

            _uow.Products.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<ProductDto>(entity);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _uow.Products.GetByIdAsync(id, ct)
                ?? throw new NotFoundException(nameof(Product), id);

            _uow.Products.Remove(entity);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<ItemDto>> GetItemsForProductAsync(int productId, CancellationToken ct = default)
        {
            var exists = await _uow.Products.ExistsAsync(p => p.Id == productId, ct);
            if (!exists) throw new NotFoundException(nameof(Product), productId);

            var items = await _uow.Items.Query(asNoTracking: true)
                .Where(i => i.ProductId == productId)
                .ToListAsync(ct);

            return _mapper.Map<List<ItemDto>>(items);
        }

        public async Task<ItemDto> AddItemAsync(int productId, CreateItemDto dto, CancellationToken ct = default)
        {
            var exists = await _uow.Products.ExistsAsync(p => p.Id == productId, ct);
            if (!exists) throw new NotFoundException(nameof(Product), productId);

            var item = new Item { ProductId = productId, Quantity = dto.Quantity };
            await _uow.Items.AddAsync(item, ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<ItemDto>(item);
        }
    }
}