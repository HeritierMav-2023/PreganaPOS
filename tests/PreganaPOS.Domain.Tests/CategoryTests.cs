using PreganaPOS.Domain;
using PreganaPOS.Domain.Entities;
using Xunit;

namespace PreganaPOS.Domain.Tests;

public sealed class CategoryTests
{
    [Fact]
    public void Constructor_WithValidData_CreatesCategoryWithoutProducts()
    {
        var category = new Category(1, "Boissons", "Produits à boire");

        Assert.Equal(1, category.Id);
        Assert.Equal("Boissons", category.Name);
        Assert.Equal("Produits à boire", category.Description);
        Assert.Empty(category.Products);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithMissingName_Throws(string? name)
    {
        var exception = Assert.ThrowsAny<ArgumentException>(() => new Category(1, name!));

        Assert.Equal("name", exception.ParamName);
    }

    [Fact]
    public void Catalog_WhenAddingSameNameIgnoringCase_Throws()
    {
        var catalog = new CategoryCatalog();
        catalog.Add(new Category(1, "Boissons"));

        var exception = Assert.Throws<InvalidOperationException>(() =>
            catalog.Add(new Category(2, " boissons ")));

        Assert.Contains("existe déjà", exception.Message);
    }

    [Fact]
    public void Category_CanContainSeveralProductsFromItsCategory()
    {
        var category = new Category(1, "Boissons");
        var firstProduct = new Product(1, "SKU-001", "Café", null, 4m, 7m, 0, 1);
        var secondProduct = new Product(2, "SKU-002", "Thé", null, 3m, 5m, 0, 1);

        category.AddProduct(firstProduct);
        category.AddProduct(secondProduct);

        Assert.Collection(category.Products,
            product => Assert.Same(firstProduct, product),
            product => Assert.Same(secondProduct, product));
    }

    [Fact]
    public void Catalog_Update_WhenNewNameAlreadyExists_ThrowsAndKeepsOriginalName()
    {
        var catalog = new CategoryCatalog();
        var drinks = new Category(1, "Boissons");
        catalog.Add(drinks);
        catalog.Add(new Category(2, "Épicerie"));

        Assert.Throws<InvalidOperationException>(() => catalog.Update(drinks, "Épicerie", null));
        Assert.Equal("Boissons", drinks.Name);
    }
}
