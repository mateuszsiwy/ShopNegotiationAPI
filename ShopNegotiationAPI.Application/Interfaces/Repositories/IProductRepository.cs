using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Interfaces.Repositories;

public interface IProductRepository
{ 
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product> GetProductByIdAsync(int id);
    Task<Product> AddProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
}