using Application.DTOs;
using Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/products")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductDto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize, ct);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken ct)
        {
            var product = await _productService.GetByIdAsync(id, ct);
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto, CancellationToken ct)
        {
            var created = await _productService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id, version = "1.0" }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken ct)
        {
            var updated = await _productService.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            await _productService.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpGet("{id:int}/items")]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<ItemDto>>> GetItems(int id, CancellationToken ct)
        {
            var items = await _productService.GetItemsForProductAsync(id, ct);
            return Ok(items);
        }

        [HttpPost("{id:int}/items")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ItemDto>> AddItem(int id, [FromBody] CreateItemDto dto, CancellationToken ct)
        {
            var item = await _productService.AddItemAsync(id, dto, ct);
            return CreatedAtAction(nameof(GetItems), new { id, version = "1.0" }, item);
        }
    }
}