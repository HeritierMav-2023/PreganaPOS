using PreganaPOS.Domain;
using PreganaPOS.Domain.Entities;
using Xunit;

namespace PreganaPOS.Domain.Tests;

public sealed class CommerceTests
{
    [Fact]
    public void Expense_RequiresDescriptionAndPositiveAmount_AndCanBeFilteredByPeriod()
    {
        Assert.Throws<ArgumentException>(() => new Expense(1, " ", 10m, new DateOnly(2025, 1, 1)));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Expense(1, "Loyer", 0m, new DateOnly(2025, 1, 1)));

        var expenses = new[]
        {
            new Expense(1, "Loyer", 100m, new DateOnly(2025, 1, 1)),
            new Expense(2, "Transport", 20m, new DateOnly(2025, 2, 1))
        };

        var result = Expense.FilterByPeriod(expenses, new DateOnly(2025, 1, 1), new DateOnly(2025, 1, 31));

        Assert.Single(result);
    }

    [Fact]
    public void Inventory_RecordsEntriesAndExits_AndRejectsExitAboveStock()
    {
        var product = CreateProduct(stock: 5);
        var inventory = new Inventory();

        inventory.RecordEntry(1, product, 3, DateTime.UtcNow, "Achat fournisseur");
        inventory.RecordExit(2, product, 4, DateTime.UtcNow, "Vente");

        Assert.Equal(4, product.QuantityInStock);
        Assert.Equal(2, inventory.Movements.Count);
        Assert.Throws<InvalidOperationException>(() =>
            inventory.RecordExit(3, product, 5, DateTime.UtcNow, "Vente"));
    }

    [Fact]
    public void Purchase_RequiresAnItem_AndIncreasesStockAfterValidation()
    {
        var product = CreateProduct(stock: 2);
        var purchase = new Purchase(1, DateTime.UtcNow, "Fournisseur");
        purchase.AddItem(product.Id, 4, 3m);

        purchase.Validate([product]);

        Assert.Equal(6, product.QuantityInStock);
        Assert.Equal(12m, purchase.TotalAmount);
        Assert.True(purchase.IsValidated);
    }

    [Fact]
    public void Sale_CalculatesTotal_DecreasesStock_AndCannotBeModifiedAfterValidation()
    {
        var product = CreateProduct(stock: 5, salePrice: 10m);
        var sale = new Sale(1, "V-001", DateTime.UtcNow, 12, PaymentMethod.Cash);
        sale.AddItem(product, 2, 1m);

        sale.Validate([product]);

        Assert.Equal(19m, sale.TotalAmount);
        Assert.Equal(3, product.QuantityInStock);
        Assert.Equal(SaleStatus.Validated, sale.Status);
        Assert.Throws<InvalidOperationException>(() => sale.AddItem(product, 1));
    }

    [Fact]
    public void Sale_WithoutItemsOrEnoughStock_IsRejected()
    {
        var product = CreateProduct(stock: 1);
        var emptySale = new Sale(1, "V-001", DateTime.UtcNow, null, PaymentMethod.Card);
        Assert.Throws<InvalidOperationException>(() => emptySale.Validate([product]));

        var sale = new Sale(2, "V-002", DateTime.UtcNow, null, PaymentMethod.Card);
        sale.AddItem(product, 2);
        Assert.Throws<InvalidOperationException>(() => sale.Validate([product]));
        Assert.Equal(1, product.QuantityInStock);
    }

    [Fact]
    public void SaleReturn_ReintegratesStockAndCalculatesRefund()
    {
        var product = CreateProduct(stock: 5, salePrice: 10m);
        var sale = new Sale(1, "V-001", DateTime.UtcNow, null, PaymentMethod.Cash);
        sale.AddItem(product, 2, 2m);
        sale.Validate([product]);

        var saleReturn = new SaleReturn(1, sale.Id, DateTime.UtcNow);
        saleReturn.AddItem(product.Id, 1);
        saleReturn.Validate(sale, [product]);

        Assert.Equal(4, product.QuantityInStock);
        Assert.Equal(9m, saleReturn.RefundedAmount);
    }

    private static Product CreateProduct(int stock, decimal salePrice = 7m) =>
        new(1, "SKU-001", "Produit", null, 4m, salePrice, stock, 1);
}
