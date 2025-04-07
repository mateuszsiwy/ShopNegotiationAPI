using Moq;
using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Application.Services;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Test.Application.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _service = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetProductByIdAsync_ExistingProduct_ReturnsProduct()
    {
        // Arrange
        var expectedProduct = new Product { Id = 1, ProductName = "Test Product" };
        _mockRepository.Setup(r => r.GetProductByIdAsync(1))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        Assert.Equal(expectedProduct, result);
        _mockRepository.Verify(r => r.GetProductByIdAsync(1), Times.Once);
    }

    

    [Fact]
    public async Task AddProductAsync_ValidProduct_ReturnsAddedProduct()
    {
        // Arrange
        var product = new Product 
        { 
            ProductName = "New Product", 
            Price = 100m, 
            Quantity = 10 
        };
            
        var addedProduct = new Product
        {
            Id = 1,
            ProductName = "New Product",
            Price = 100m,
            Quantity = 10
        };
            
        _mockRepository.Setup(r => r.AddProductAsync(product))
            .ReturnsAsync(addedProduct);

        // Act
        var result = await _service.AddProductAsync(product);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("New Product", result.ProductName);
        _mockRepository.Verify(r => r.AddProductAsync(product), Times.Once);
    }

    [Fact]
    public async Task ProductExistsAsync_ExistingProduct_ReturnsTrue()
    {
        // Arrange
        _mockRepository.Setup(r => r.ProductExistsAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _service.ProductExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ProductExistsAsync_NonExistingProduct_ReturnsFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.ProductExistsAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _service.ProductExistsAsync(999);

        // Assert
        Assert.False(result);
    }

}