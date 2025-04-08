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
public class NegotiationsControllerIntegrationTests
{
    private readonly AppDbContext _context;
    private readonly NegotiationsController _controller;
    private readonly NegotiationRepository _negotiationRepository;
    private readonly ProductRepository _productRepository;

    public NegotiationsControllerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new AppDbContext(options);
        _negotiationRepository = new NegotiationRepository(_context, Mock.Of<ILogger<NegotiationRepository>>());
        _productRepository = new ProductRepository(_context, Mock.Of<ILogger<ProductRepository>>());

        var mockNegotiationService = new Mock<INegotiationService>();
        var mockProductService = new Mock<IProductService>();
        var mockLogger = new Mock<ILogger<NegotiationsController>>();
        _controller = new NegotiationsController(mockNegotiationService.Object, mockProductService.Object, mockLogger.Object);
        InitializeDatabase();

    }
    private void InitializeDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateNegotiation_ValidNegotiation_AddsNegotiation()
    {
        // Arrange
        var product = new Product { Id = 1, ProductName = "Product1", Price = 200 };
        await _productRepository.AddProductAsync(product);
    
        var negotiationDto = new NegotiationDTO 
        { 
            ProductId = 1, 
            ProposedPrice = 100, 
            NegotiatorName = "John Doe" 
        };
        
        var negotiation = new Negotiation
        {
            Id = 1,
            ProductId = 1,
            ProposedPrice = 100,
            NegotiatorName = "John Doe",
            Status = NegotiationStatus.Pending 
        };
    
        var mockNegotiationService = new Mock<INegotiationService>();
        mockNegotiationService
            .Setup(service => service.AddNegotiationAsync(It.IsAny<Negotiation>()))
            .ReturnsAsync(negotiation);
    
        var mockProductService = new Mock<IProductService>();
        mockProductService
            .Setup(service => service.ProductExistsAsync(1))
            .ReturnsAsync(true);
        mockProductService
            .Setup(service => service.GetProductByIdAsync(1))
            .ReturnsAsync(product);
    
        var controller = new NegotiationsController(
            mockNegotiationService.Object,
            mockProductService.Object,
            Mock.Of<ILogger<NegotiationsController>>()
        );
    
        // Act
        var result = await controller.CreateNegotiation(negotiationDto);
    
        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<Negotiation>(createdAtActionResult.Value);
        Assert.Equal(1, returnValue.Id);
    }
}