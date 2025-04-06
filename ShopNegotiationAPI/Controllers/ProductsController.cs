﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNegotiationAPI.Application.Interfaces.Services;
using ShopNegotiationAPI.Domain.Models;

namespace ShopNegotiationAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
    {
        return Ok(await _productService.GetAllProductsAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        return Ok(await _productService.GetProductByIdAsync(id));
    }

    [HttpPost]
    [Authorize(Policy = "EmployeesOnly")]
    public async Task<ActionResult<Product>> CreateProduct(Product product)
    {
        var result = await _productService.AddProductAsync(product);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }
}