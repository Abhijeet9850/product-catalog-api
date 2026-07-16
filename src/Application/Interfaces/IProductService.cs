using Application.DTOs;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetAllAsync(int pageNumber, int pageSize, CancellationToken ct = default);

        Task<ProductDto> GetByIdAsync(int id, CancellationToken ct = default);

        Task<ProductDto> CreateAsync(CreateProductDto dto, CancellationToken ct = default);

        Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto, CancellationToken ct = default);

        Task DeleteAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<ItemDto>> GetItemsForProductAsync(int productId, CancellationToken ct = default);

        Task<ItemDto> AddItemAsync(int productId, CreateItemDto dto, CancellationToken ct = default);
    }
}