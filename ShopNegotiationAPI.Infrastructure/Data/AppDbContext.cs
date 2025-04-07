using Microsoft.EntityFrameworkCore;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Negotiation> Negotiations { get; set; }
    public DbSet<User> Users { get; set; }
}