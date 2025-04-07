using Microsoft.Extensions.Logging;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Infrastructure.Data;

public class InitializeDatabase
{
    private readonly AppDbContext _context;
    private readonly ILogger<InitializeDatabase> _logger;

    public InitializeDatabase(AppDbContext context, ILogger<InitializeDatabase> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        if (await _context.Database.EnsureCreatedAsync())
            _logger.LogInformation("Database created.");
        else
            _logger.LogInformation("Database already exists.");
        await ClearDatabaseAsync(); 
        await SeedDataAsync();
    }

    private async Task ClearDatabaseAsync()
    {
        _logger.LogInformation("Clearing database...");
        _context.Products.RemoveRange(_context.Products);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Database cleared.");
    }

    private async Task SeedDataAsync()
    {
        if (!_context.Products.Any())
        {
            _logger.LogInformation("Seeding data...");
            var products = new List<Product>
            {
                new()
                {
                    ProductName = "Laptop", Description = "High performance laptop", Price = 1200.00m, Quantity = 10
                },
                new()
                {
                    ProductName = "Smartphone", Description = "Latest model smartphone", Price = 800.00m, Quantity = 20
                },
                new()
                {
                    ProductName = "Headphones", Description = "Noise-cancelling headphones", Price = 200.00m,
                    Quantity = 30
                }
            };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Saved {Count} entities to in-memory store.", products.Count);
        }
        else
        {
            _logger.LogInformation("Products table already contains data.");
        }
    }
}