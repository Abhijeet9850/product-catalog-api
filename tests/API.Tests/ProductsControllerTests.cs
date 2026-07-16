using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace API.Tests
{
    public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductsControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_ReturnsSuccessStatusCode()
        {
            var response = await _client.GetAsync("/api/v1/products?pageNumber=1&pageSize=10");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_UnknownId_ReturnsNotFound()
        {
            var response = await _client.GetAsync("/api/v1/products/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_WithoutAuth_ReturnsUnauthorized()
        {
            var payload = new CreateProductDto { ProductName = "Test", CreatedBy = "tester" };

            var response = await _client.PostAsJsonAsync("/api/v1/products", payload);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
