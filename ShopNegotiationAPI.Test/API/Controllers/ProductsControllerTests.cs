using Microsoft.AspNetCore.Mvc;
using Moq;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Controllers;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.DTOs;
namespace ShopNegotiationAPI.Test.API.Controllers;
public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductsController(_mockProductService.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResultWithProducts()
    {
        // Arrange
        var products = new List<Product> { new() { Id = 1, ProductName = "Product1" } };
        _mockProductService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetProduct_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Product1" };
        _mockProductService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task CreateProduct_ValidProduct_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var productDto = new ProductDTO { ProductName = "New Product", Price = 100m, Quantity = 10 };
        var product = new Product { Id = 1, ProductName = "New Product", Price = 100m, Quantity = 10 };

        _mockProductService.Setup(s => s.AddProductAsync(It.IsAny<Product>())).ReturnsAsync(product);

        // Act
        var result = await _controller.CreateProduct(productDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<Product>(createdAtActionResult.Value);
        Assert.Equal(1, returnValue.Id);
    }
}