﻿using ShopNegotiationAPI.Application.Interfaces.Repositories;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllProductsAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _productRepository.GetProductByIdAsync(id);
    }

    public async Task<Product> AddProductAsync(Product product)
    {
        return await _productRepository.AddProductAsync(product);
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        return await _productRepository.UpdateProductAsync(product);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _productRepository.DeleteProductAsync(id);
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        return await _productRepository.ProductExistsAsync(id);
    }
}