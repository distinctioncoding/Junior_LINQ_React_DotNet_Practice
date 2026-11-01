using Microsoft.AspNetCore.Mvc;
using Practice.Api.Models;

namespace Practice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static readonly List<Product> Products =
    [
        new() { Id = 1, Name = "Laptop", Price = 1200, Category = "Tech" },
        new() { Id = 2, Name = "Mouse", Price = 30, Category = "Tech" },
        new() { Id = 3, Name = "Chair", Price = 200, Category = "Office" },
        new() { Id = 4, Name = "Desk", Price = 500, Category = "Office" },
        new() { Id = 5, Name = "Keyboard", Price = 80, Category = "Tech" }
    ];

    // GET: api/products
    [HttpGet]
    public ActionResult<List<Product>> GetAll()
    {
        return Ok(Products);
    }

    // GET: api/products/search?term=lap
    [HttpGet("search")]
    public ActionResult<List<Product>> Search(string? term)
    {
        // An empty search box means "show everything"
        if (string.IsNullOrWhiteSpace(term))
        {
            return Ok(Products);
        }

        var result = Products
            .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(result);
    }

    // GET: api/products/category/Tech
    [HttpGet("category/{category}")]
    public ActionResult<List<Product>> GetByCategory(string category)
    {
        var result = Products
            .Where(p => p.Category == category)
            .ToList();

        return Ok(result);
    }

    // GET: api/products/cheap
    [HttpGet("cheap")]
    public ActionResult<List<Product>> GetCheapProducts()
    {
        var result = Products
            .Where(p => p.Price < 300)
            .OrderBy(p => p.Price)
            .ToList();

        return Ok(result);
    }

    // GET: api/products/expensive
    [HttpGet("expensive")]
    public ActionResult<Product?> GetMostExpensive()
    {
        var result = Products
            .OrderByDescending(p => p.Price)
            .FirstOrDefault();

        return Ok(result);
    }
}
