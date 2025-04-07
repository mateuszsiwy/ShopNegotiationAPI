using Microsoft.Extensions.Configuration;
using Moq;
using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Application.Services;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Test.Application.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("TestSecretKeyWithAtLeast32Characters!!");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_mockUserRepository.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task ValidateUser_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = hashedPassword,
            Role = "User"
        };

        _mockUserRepository.Setup(r => r.GetByUsername("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ValidateUser("testuser", "password123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task ValidateUser_WithInvalidPassword_ReturnsNull()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            PasswordHash = hashedPassword,
            Role = "User"
        };

        _mockUserRepository.Setup(r => r.GetByUsername("testuser"))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.ValidateUser("testuser", "wrongpassword");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task ValidateUser_WithNonExistentUser_ReturnsNull()
    {
        // Arrange
        _mockUserRepository.Setup(r => r.GetByUsername("nonexistent"))
            .ReturnsAsync((User)null);

        // Act
        var result = await _authService.ValidateUser("nonexistent", "password");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GenerateJwtToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Username = "testuser",
            Role = "User"
        };

        // Act
        var token = _authService.GenerateJwtToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }
}