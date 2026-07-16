using Application.DTOs;
using Application.Interfaces;
using Application.Mapping;
using Application.Services;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.Tests
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IProductRepository> _productRepoMock = new();
        private readonly Mock<IItemRepository> _itemRepoMock = new();
        private readonly IMapper _mapper;
        private readonly ProductService _sut;

        public ProductServiceTests()
        {
            _uowMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
            _uowMock.Setup(u => u.Items).Returns(_itemRepoMock.Object);

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = mapperConfig.CreateMapper();

            _sut = new ProductService(_uowMock.Object, _mapper);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingProduct_ReturnsMappedDto()
        {
            var product = new Product { Id = 1, ProductName = "Widget", CreatedBy = "abhijeet", CreatedOn = DateTime.UtcNow };
            _productRepoMock.Setup(r => r.GetWithItemsAsync(1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            var result = await _sut.GetByIdAsync(1);

            result.Id.Should().Be(1);
            result.ProductName.Should().Be("Widget");
        }

        [Fact]
        public async Task GetByIdAsync_MissingProduct_ThrowsNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetWithItemsAsync(99, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var act = async () => await _sut.GetByIdAsync(99);

            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task CreateAsync_ValidDto_AddsProductAndSaves()
        {
            var dto = new CreateProductDto { ProductName = "New Product", CreatedBy = "abhijeet" };

            var result = await _sut.CreateAsync(dto);

            result.ProductName.Should().Be("New Product");
            _productRepoMock.Verify(r => r.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_MissingProduct_ThrowsNotFoundException()
        {
            _productRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Product?)null);

            var act = async () => await _sut.DeleteAsync(5);

            await act.Should().ThrowAsync<NotFoundException>();
        }
    }
}
