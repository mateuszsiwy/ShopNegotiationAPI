using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShopNegotiationAPI.Application.Exceptions;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Controllers;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.DTOs;
namespace ShopNegotiationAPI.Test.API.Controllers;

public class NegotiationsControllerTests
{
    private readonly Mock<INegotiationService> _mockNegotiationService;
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<NegotiationsController>> _mockLogger;
    private readonly NegotiationsController _controller;

    public NegotiationsControllerTests()
    {
        _mockNegotiationService = new Mock<INegotiationService>();
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<NegotiationsController>>();
        _controller = new NegotiationsController(_mockNegotiationService.Object, _mockProductService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetNegotiation_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var negotiation = new Negotiation { Id = 1 };
        _mockNegotiationService.Setup(s => s.GetNegotiationByIdAsync(1)).ReturnsAsync(negotiation);

        // Act
        var result = await _controller.GetNegotiation(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<Negotiation>(okResult.Value);
        Assert.Equal(1, returnValue.Id);
    }

    [Fact]
    public async Task GetNegotiation_NonExistingId_ReturnsNotFoundResult()
    {
        // Arrange
        _mockNegotiationService.Setup(s => s.GetNegotiationByIdAsync(1))
            .ThrowsAsync(new NegotiationNotFoundException(1));

        // Act
        var result = await _controller.GetNegotiation(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateNegotiation_ValidNegotiation_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var negotiationDto = new NegotiationDTO { ProductId = 1, ProposedPrice = 100, NegotiatorName = "John Doe" };
        var product = new Product { Id = 1, Price = 200 };
        var negotiation = new Negotiation
            { Id = 1, ProductId = 1, ProposedPrice = 100, InitialPrice = 200, NegotiatorName = "John Doe" };

        _mockProductService.Setup(s => s.ProductExistsAsync(1)).ReturnsAsync(true);
        _mockNegotiationService.Setup(s => s.AddNegotiationAsync(It.IsAny<Negotiation>())).ReturnsAsync(negotiation);

        // Act
        _mockProductService.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);
        var result = await _controller.CreateNegotiation(negotiationDto);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnValue = Assert.IsType<Negotiation>(createdAtActionResult.Value);
        Assert.Equal(1, returnValue.Id);
    }
}