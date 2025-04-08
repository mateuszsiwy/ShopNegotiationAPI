using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
public class AuthControllerIntegrationTests
{
    private readonly AppDbContext _context;
    private readonly AuthController _controller;
    private readonly UserRepository _userRepository;

    public AuthControllerIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new AppDbContext(options);
        _userRepository = new UserRepository(_context);

        var mockAuthService = new Mock<IAuthService>();
        var mockConfiguration = new Mock<IConfiguration>();
        _controller = new AuthController(mockAuthService.Object, mockConfiguration.Object);
        InitializeDatabase();

    }
    private void InitializeDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task Register_ValidRequest_AddsUser()
    {
        // Arrange
        var request = new RegisterRequest { Username = "newuser", Password = "password" };
    
        var mockAuthService = new Mock<IAuthService>();
        mockAuthService
            .Setup(service => service.RegisterUser(It.IsAny<User>()))
            .Callback<User>(user => 
            {
                // This simulates what the actual service would do
                _context.Users.Add(user);
                _context.SaveChanges();
            });
    
        var mockConfiguration = new Mock<IConfiguration>();
        var controller = new AuthController(mockAuthService.Object, mockConfiguration.Object);
    
        // Act
        var result = await controller.Register(request);
    
        // Assert
        Assert.IsType<OkResult>(result);
        Assert.Equal(1, _context.Users.Count());
    }
}