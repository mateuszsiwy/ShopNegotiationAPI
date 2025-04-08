using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.DTOs;

namespace ShopNegotiationAPI.Test.API.Controllers;
public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _controller = new AuthController(_mockAuthService.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkResult()
    {
        // Arrange
        var request = new LoginRequest { Username = "testuser", Password = "password" };
        var user = new User { Id = 1, Username = "testuser" };
        _mockAuthService.Setup(s => s.ValidateUser(request.Username, request.Password)).ReturnsAsync(user);
        _mockAuthService.Setup(s => s.GenerateJwtToken(user)).Returns("token");
    
        // Act
        var result = await _controller.Login(request);
    
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = okResult.Value.GetType().GetProperty("Token").GetValue(okResult.Value, null);
        Assert.Equal("token", returnValue);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorizedResult()
    {
        // Arrange
        var request = new LoginRequest { Username = "testuser", Password = "wrongpassword" };
        _mockAuthService.Setup(s => s.ValidateUser(request.Username, request.Password)).ReturnsAsync((User)null);

        // Act
        var result = await _controller.Login(request);

        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task Register_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new RegisterRequest { Username = "newuser", Password = "password" };
        var user = new User
        {
            Username = request.Username, PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password), Role = "User"
        };

        // Act
        var result = await _controller.Register(request);

        // Assert
        Assert.IsType<OkResult>(result);
        _mockAuthService.Verify(s => s.RegisterUser(It.Is<User>(u => u.Username == request.Username)), Times.Once);
    }
}