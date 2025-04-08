using Microsoft.EntityFrameworkCore;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.Infrastructure.Data;
using ShopNegotiationAPI.Infrastructure.Repositories;
using Xunit;
namespace ShopNegotiationAPI.Test.Infrastructure.Repositories;

public class UserRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly UserRepository _repository;

    public UserRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabaseUserRepository")
            .Options;
        _context = new AppDbContext(options);
        _repository = new UserRepository(_context);
        InitializeDatabase();

    }
    private void InitializeDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetByUsername_ExistingUsername_ReturnsUser()
    {
        InitializeDatabase();
        // Arrange
        var user = new User { Id = 1, Username = "testuser" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUsername("testuser");
        var testString = "testuser";
        // Assert
        Assert.Equal(testString, result.Username);
    }

    [Fact]
    public async Task GetByUsername_NonExistingUsername_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByUsername("nonexistinguser");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddUser_ValidUser_AddsUser()
    {
        // Arrange
        var user = new User { Id = 1, Username = "newuser" };

        // Act
        await _repository.AddUser(user);

        // Assert
        Assert.Equal(1, _context.Users.Count());
    }
}