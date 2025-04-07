using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopNegotiationAPI.Application.Exceptions;
using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Domain.Models;
using ShopNegotiationAPI.Infrastructure.Data;

namespace ShopNegotiationAPI.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ProductRepository> _logger;
    public ProductRepository(AppDbContext context, ILogger<ProductRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        try
        {
            return await _context.Products.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving products");
            throw;
        }
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id {Id} not found", id);
                throw new ProductNotFoundException(id);
            }
            return product;
        } 
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving product with id {Id}", id);
            throw;
        }
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        try
        {
            if (product == null)
            {
                _logger.LogWarning("Cannot add product. Product object is null");
                throw new ArgumentNullException(nameof(product));
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error adding product");
            throw new Exception("An error occurred while adding the product.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product");
            throw;
        }
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        try
        {
            if (product == null)
            {
                _logger.LogWarning("Cannot update product. Product object is null");
                throw new ArgumentNullException(nameof(product));
            }

            var existingProduct = await _context.Products.FindAsync(product.Id);
            if (existingProduct == null)
            {
                _logger.LogWarning("Product with id {Id} not found", product.Id);
                throw new ProductNotFoundException(product.Id);
            }

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();
            return product;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error updating product");
            throw new Exception("An error occurred while updating the product.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product");
            throw;
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product with id {Id} not found", id);
                throw new ProductNotFoundException(id);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error deleting product");
            throw new Exception("An error occurred while deleting the product.", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product");
            throw;
        }
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        try
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if product exists");
            throw;
        }
    }
}