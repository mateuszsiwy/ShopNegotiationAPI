using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Controllers;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.DTOs;
using ShopNegotiationAPI.Infrastructure.Data;
using ShopNegotiationAPI.Infrastructure.Repositories;
using Xunit;
namespace ShopNegotiationAPI.Test.IntegrationTests;
public class ProductsControllerIntegrationTests
{
    private readonly AppDbContext _context;
    private readonly ProductsController _controller;
    private readonly ProductRepository _repository;

    public ProductsControllerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new AppDbContext(options);
        _repository = new ProductRepository(_context, Mock.Of<ILogger<ProductRepository>>());

        var mockProductService = new Mock<IProductService>();
        _controller = new ProductsController(mockProductService.Object);
        InitializeDatabase();

    }
    private void InitializeDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateProduct_ValidProduct_AddsProduct()
    {
        // Arrange
        var productDto = new ProductDTO { ProductName = "New Product", Price = 100m, Quantity = 10 };
        var product = new Product { Id = 1, ProductName = "New Product", Price = 100m, Quantity = 10 };
    
        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(service => service.AddProductAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);
    
        var controller = new ProductsController(mockProductService.Object);
    
        // Act
        var result = await controller.CreateProduct(productDto);
    
        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<Product>(createdAtActionResult.Value);
        Assert.Equal(1, returnValue.Id);
        Assert.Equal("New Product", returnValue.ProductName);
    }
}