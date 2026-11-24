using Xunit;
using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Domain.Tests;

public sealed class ProductTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesActiveProductWithEmptyStock()
    {
        var product = new Product(1, "SKU-001", "Café", "Café moulu", 4.50m, 7m, 0, 2);

        Assert.Equal(1, product.Id);
        Assert.Equal("SKU-001", product.Code);
        Assert.Equal("Café", product.Name);
        Assert.Equal(0, product.QuantityInStock);
        Assert.True(product.IsActive);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithMissingName_Throws(string? name)
    {
        var exception = Assert.ThrowsAny<ArgumentException>(() =>
            new Product(1, "SKU-001", name!, null, 4.50m, 7m, 0, 2));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNegativeSalePrice_Throws()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Product(1, "SKU-001", "Café", null, 4.50m, -0.01m, 0, 2));

        Assert.Equal("salePrice", exception.ParamName);
    }

    [Fact]
    public void Constructor_WithNegativeStock_Throws()
    {
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Product(1, "SKU-001", "Café", null, 4.50m, 7m, -1, 2));

        Assert.Equal("quantityInStock", exception.ParamName);
    }

    [Fact]
    public void Update_WithValidData_ChangesProductDetails()
    {
        var product = new Product(1, "SKU-001", "Café", null, 4.50m, 7m, 0, 2);

        product.Update("SKU-002", "Thé", "Thé vert", 3m, 5m, 12, 3);

        Assert.Equal("SKU-002", product.Code);
        Assert.Equal("Thé", product.Name);
        Assert.Equal("Thé vert", product.Description);
        Assert.Equal(3m, product.PurchasePrice);
        Assert.Equal(5m, product.SalePrice);
        Assert.Equal(12, product.QuantityInStock);
        Assert.Equal(3, product.CategoryId);
    }

    [Fact]
    public void DeactivateThenActivate_ChangesProductStatus()
    {
        var product = new Product(1, "SKU-001", "Café", null, 4.50m, 7m, 0, 2);

        product.Deactivate();
        Assert.False(product.IsActive);

        product.Activate();
        Assert.True(product.IsActive);
    }
}
