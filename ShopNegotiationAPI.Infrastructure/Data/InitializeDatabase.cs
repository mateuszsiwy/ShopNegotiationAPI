using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Infrastructure.Data;

public class InitializeDatabase
{
    private readonly AppDbContext _context;

    public InitializeDatabase(AppDbContext context)
    {
        _context = context;
    }
    public void Initialize()
    {
        if (_context.Database.EnsureCreated())
        {
            SeedData();
        }
    }
    private void SeedData()
    {
        if (!_context.Products.Any())
        {
            var products = new List<Product>
            {
                new Product { ProductName = "Laptop", Description = "High performance laptop", Price = 1200.00m, Quantity = 10 },
                new Product { ProductName = "Smartphone", Description = "Latest model smartphone", Price = 800.00m, Quantity = 20 },
                new Product { ProductName = "Headphones", Description = "Noise-cancelling headphones", Price = 200.00m, Quantity = 30 }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }
    }
}