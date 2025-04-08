using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ShopNegotiationAPI.Application.Exceptions;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.Infrastructure.Data;
using ShopNegotiationAPI.Infrastructure.Repositories;
namespace ShopNegotiationAPI.Test.Infrastructure.Repositories;
public class NegotiationRepositoryTests
{
    private readonly AppDbContext _context;
    private readonly NegotiationRepository _repository;
    private readonly Mock<ILogger<NegotiationRepository>> _mockLogger;

    public NegotiationRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabaseNegotiationRepository")
            .Options;
        _context = new AppDbContext(options);
        _mockLogger = new Mock<ILogger<NegotiationRepository>>();
        _repository = new NegotiationRepository(_context, _mockLogger.Object);
        InitializeDatabase();

    }
    private void InitializeDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAllNegotiationsAsync_ReturnsAllNegotiations()
    {
        // Arrange
        var negotiations = new List<Negotiation>
        {
            new() { Id = 4 },
            new() { Id = 5 }
        };
        _context.Negotiations.AddRange(negotiations);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllNegotiationsAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetNegotiationByIdAsync_ExistingId_ReturnsNegotiation()
    {
        // Arrange
        var negotiation = new Negotiation { Id = 3 };
        _context.Negotiations.Add(negotiation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetNegotiationByIdAsync(3);

        // Assert
        Assert.Equal(3, result.Id);
    }

    [Fact]
    public async Task GetNegotiationByIdAsync_NonExistingId_ThrowsNegotiationNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NegotiationNotFoundException>(() => _repository.GetNegotiationByIdAsync(999));
    }

    [Fact]
    public async Task AddNegotiationAsync_ValidNegotiation_ReturnsNegotiation()
    {
        // Arrange
        var negotiation = new Negotiation { Id = 6 };

        // Act
        var result = await _repository.AddNegotiationAsync(negotiation);

        // Assert
        Assert.Equal(6, result.Id);
        Assert.Equal(1, _context.Negotiations.Count());
    }

    [Fact]
    public async Task UpdateNegotiationAsync_ValidNegotiation_ReturnsUpdatedNegotiation()
    {
        // Arrange
        var negotiation = new Negotiation { Id = 1, ProposedPrice = 100 };
        _context.Negotiations.Add(negotiation);
        await _context.SaveChangesAsync();
        negotiation.ProposedPrice = 200;

        // Act
        var result = await _repository.UpdateNegotiationAsync(negotiation);

        // Assert
        Assert.Equal(200, result.ProposedPrice);
    }

    [Fact]
    public async Task DeleteNegotiationAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var negotiation = new Negotiation { Id = 7 };
        _context.Negotiations.Add(negotiation);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteNegotiationAsync(7);

        // Assert
        Assert.True(result);
        Assert.Equal(0, _context.Negotiations.Count());
    }
}