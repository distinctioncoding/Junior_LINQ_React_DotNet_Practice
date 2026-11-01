using Microsoft.AspNetCore.Mvc;
using Practice.Api.Controllers;
using Practice.Api.Models;

namespace Practice.Api.Tests;

public class ProductsControllerTests
{
    private readonly ProductsController _controller = new();

    // GetAll

    [Fact]
    public void GetAll_ReturnsAllProducts()
    {
        var products = GetOkValue(_controller.GetAll());

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, products.Select(p => p.Id));
    }

    // Search

    [Theory]
    [InlineData("lap", "Laptop")]
    [InlineData("LAP", "Laptop")]
    [InlineData("board", "Keyboard")]
    [InlineData("Desk", "Desk")]
    public void Search_MatchesPartOfName_IgnoringCase(string term, string expectedName)
    {
        var products = GetOkValue(_controller.Search(term));

        var product = Assert.Single(products);
        Assert.Equal(expectedName, product.Name);
    }

    [Fact]
    public void Search_TermInSeveralNames_ReturnsEveryMatch()
    {
        var products = GetOkValue(_controller.Search("o"));

        Assert.Equal(new[] { "Laptop", "Mouse", "Keyboard" }, products.Select(p => p.Name));
    }

    [Fact]
    public void Search_NoMatch_ReturnsEmptyList()
    {
        var products = GetOkValue(_controller.Search("phone"));

        Assert.Empty(products);
    }

    [Fact]
    public void Search_EmptyTerm_ReturnsAllProducts()
    {
        var products = GetOkValue(_controller.Search(""));

        Assert.Equal(5, products.Count);
    }

    // GetByCategory

    [Fact]
    public void GetByCategory_Tech_ReturnsOnlyTechProducts()
    {
        var products = GetOkValue(_controller.GetByCategory("Tech"));

        Assert.Equal(new[] { "Laptop", "Mouse", "Keyboard" }, products.Select(p => p.Name));
        Assert.All(products, p => Assert.Equal("Tech", p.Category));
    }

    [Fact]
    public void GetByCategory_Office_ReturnsOnlyOfficeProducts()
    {
        var products = GetOkValue(_controller.GetByCategory("Office"));

        Assert.Equal(new[] { "Chair", "Desk" }, products.Select(p => p.Name));
        Assert.All(products, p => Assert.Equal("Office", p.Category));
    }

    [Fact]
    public void GetByCategory_UnknownCategory_ReturnsEmptyList()
    {
        var products = GetOkValue(_controller.GetByCategory("Garden"));

        Assert.Empty(products);
    }

    // GetCheapProducts

    [Fact]
    public void GetCheapProducts_ReturnsProductsUnder300_SortedByPriceAscending()
    {
        var products = GetOkValue(_controller.GetCheapProducts());

        Assert.Equal(new[] { "Mouse", "Keyboard", "Chair" }, products.Select(p => p.Name));
        Assert.All(products, p => Assert.True(p.Price < 300));
        Assert.Equal(products.OrderBy(p => p.Price), products);
    }

    // GetMostExpensive

    [Fact]
    public void GetMostExpensive_ReturnsHighestPricedProduct()
    {
        var product = GetOkValue(_controller.GetMostExpensive());

        Assert.NotNull(product);
        Assert.Equal("Laptop", product.Name);
        Assert.Equal(1200m, product.Price);
    }

    private static T GetOkValue<T>(ActionResult<T> result)
    {
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        return Assert.IsType<T>(ok.Value);
    }
}
