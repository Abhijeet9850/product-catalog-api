using System;
using System.Collections.Generic;

namespace Application.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public List<ItemDto> Items { get; set; } = new();
    }

    public class CreateProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
    }

    public class UpdateProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
    }

    public class ItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class CreateItemDto
    {
        public int Quantity { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}