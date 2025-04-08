using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ShopNegotiationAPI.Application.Exceptions;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.Infrastructure.Data;
using ShopNegotiationAPI.Infrastructure.Repositories;
namespace ShopNegotiationAPI.Test.Infrastructure.Repositories;

public class ProductRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly ProductRepository _repository;
    private readonly Mock<ILogger<ProductRepository>> _mockLogger;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabaseProductRepository")
            .Options;
        _context = new AppDbContext(options);
        _mockLogger = new Mock<ILogger<ProductRepository>>();
        _repository = new ProductRepository(_context, _mockLogger.Object);
        InitializeDatabase();

    }
    private void InitializeDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }
    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, ProductName = "Product1" },
            new() { Id = 2, ProductName = "Product2" }
        };
        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllProductsAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProductByIdAsync_ExistingId_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Product1" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetProductByIdAsync(1);

        // Assert
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetProductByIdAsync_NonExistingId_ThrowsProductNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() => _repository.GetProductByIdAsync(999));
    }

    [Fact]
    public async Task AddProductAsync_ValidProduct_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Product1" };

        // Act
        var result = await _repository.AddProductAsync(product);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal(1, _context.Products.Count());
    }

    [Fact]
    public async Task UpdateProductAsync_ValidProduct_ReturnsUpdatedProduct()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Product1" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        product.ProductName = "UpdatedProduct";

        // Act
        var result = await _repository.UpdateProductAsync(product);

        // Assert
        Assert.Equal("UpdatedProduct", result.ProductName);
    }

    [Fact]
    public async Task DeleteProductAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Product1" };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteProductAsync(1);

        // Assert
        Assert.True(result);
        Assert.Equal(0, _context.Products.Count());
    }
}