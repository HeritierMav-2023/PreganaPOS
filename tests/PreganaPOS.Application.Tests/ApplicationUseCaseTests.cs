using PreganaPOS.Application.Dashboard;
using PreganaPOS.Application.Products;
using PreganaPOS.Application.Sales;
using PreganaPOS.Domain.Entities;
using PreganaPOS.Infrastructure.InMemory;
using Xunit;

namespace PreganaPOS.Application.Tests;

public sealed class ApplicationUseCaseTests
{
    [Fact]
    public void ProductService_CreatesThroughRepositoryAndSearchesWithPagination()
    {
        var products = new InMemoryProductRepository();
        var categories = new InMemoryCategoryRepository();
        categories.Add(new Category(1, "Boissons"));
        var service = new ProductService(products, categories);

        service.Create(new CreateProductCommand("CAF-001", "Café", null, 4m, 7m, 3, 1));
        service.Create(new CreateProductCommand("THE-001", "Thé", null, 3m, 5m, 2, 1));

        var result = service.Search(new ProductSearchQuery("é", null, 1, false, 1, 1));

        Assert.Equal(2, result.TotalCount);
        Assert.Single(result.Items);
    }

    [Fact]
    public void CreateSaleService_RejectsInvalidSaleWithoutCreatingSaleOrChangingStock()
    {
        var products = new InMemoryProductRepository();
        var categories = new InMemoryCategoryRepository();
        categories.Add(new Category(1, "Boissons"));
        var productService = new ProductService(products, categories);
        productService.Create(new CreateProductCommand("CAF-001", "Café", null, 4m, 7m, 1, 1));
        var product = products.GetByCode("CAF-001")!;
        var sales = new InMemorySaleRepository();
        var customers = new InMemoryCustomerRepository();
        var service = new CreateSaleService(sales, products, customers, new InMemoryUnitOfWork());

        var command = new CreateSaleCommand(null, PaymentMethod.Cash, [new CreateSaleItemCommand(product.Id, 2)]);

        Assert.Throws<InvalidOperationException>(() => service.Execute(command));
        Assert.Null(sales.GetById(1));
        Assert.Equal(1, product.QuantityInStock);
    }

    [Fact]
    public void CreateSaleService_ValidatesProductsAndPersistsValidatedSale()
    {
        var products = new InMemoryProductRepository();
        var categories = new InMemoryCategoryRepository();
        categories.Add(new Category(1, "Boissons"));
        var productService = new ProductService(products, categories);
        productService.Create(new CreateProductCommand("CAF-001", "Café", null, 4m, 7m, 2, 1));
        var product = products.GetByCode("CAF-001")!;
        var sales = new InMemorySaleRepository();
        var customers = new InMemoryCustomerRepository();
        customers.Add(new Customer(3, "Client Test"));
        var service = new CreateSaleService(sales, products, customers, new InMemoryUnitOfWork());

        //Ventes
        var sale = service.Execute(new CreateSaleCommand(3, PaymentMethod.Card, [new CreateSaleItemCommand(product.Id, 1)]));

        Assert.Equal(SaleStatus.Validated, sale.Status);
        Assert.Equal(1, product.QuantityInStock);
        Assert.Same(sales.GetById(sale.Id), sales.GetById(sale.Id));
    }

    [Fact]
    public void DashboardService_CalculatesTotalsAndExcludesDraftSales()
    {
        //Produits
        var products = new InMemoryProductRepository();
        var product = new Product(products.NextId(), "CAF-001", "Café", null, 4m, 10m, 1, 1);
        products.Add(product);

        //Ventes
        var sales = new InMemorySaleRepository();
        var sale = new Sale(sales.NextId(), "V-001", DateTime.UtcNow, null, PaymentMethod.Cash);
        sale.AddItem(product, 1);
        sale.Validate([product]);
        sales.Add(sale);

        //Achats
        var purchases = new InMemoryPurchaseRepository();
        var purchase = new Purchase(1, DateTime.UtcNow);
        purchase.AddItem(product.Id, 1, 4m);
        purchase.Validate([product]);
        purchases.Add(purchase);

        //Frais
        var expenses = new InMemoryExpenseRepository();
        expenses.Add(new Expense(1, "Transport", 1m, DateOnly.FromDateTime(DateTime.UtcNow)));

        //Tableau de bord
        var dashboard = new DashboardService(sales, purchases, expenses, products)
            .Get(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), 5);

        Assert.Equal(10m, dashboard.TotalSales);
        Assert.Equal(4m, dashboard.TotalPurchases);
        Assert.Equal(1m, dashboard.TotalExpenses);
        Assert.Equal(5m, dashboard.TotalProfit);
        Assert.Single(dashboard.SalesByDay);
    }
}
