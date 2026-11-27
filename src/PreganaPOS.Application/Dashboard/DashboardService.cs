using PreganaPOS.Application.Abstractions;
using PreganaPOS.Application.Products;
using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Application.Dashboard;


#region Dashboard DTO

public sealed record DashboardDto(
    decimal TotalSales,
    decimal TotalPurchases,
    decimal TotalExpenses,
    decimal TotalProfit,
    IReadOnlyCollection<ProductDto> LowStockProducts,
    IReadOnlyDictionary<DateOnly, decimal> SalesByDay);

#endregion

#region Dashboard Service


public sealed class DashboardService(
    ISaleRepository sales,
    IPurchaseRepository purchases,
    IExpenseRepository expenses,
    IProductRepository products)
{
    public DashboardDto Get(DateTime start, DateTime end, int lowStockThreshold = 5)
    {
        if (start > end)
        {
            throw new ArgumentException("La date de début doit précéder la date de fin.");
        }

        if (lowStockThreshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lowStockThreshold));
        }

        var saleList = sales.GetBetween(start, end).Where(sale => sale.Status == SaleStatus.Validated).ToArray();
        var purchaseList = purchases.GetBetween(start, end).ToArray();
        var expenseList = expenses.GetBetween(DateOnly.FromDateTime(start), DateOnly.FromDateTime(end)).ToArray();
        var lowStock = products.Search(null, null, null, false)
            .Where(product => product.QuantityInStock <= lowStockThreshold)
            .Select(product => new ProductDto(
                product.Id, product.Code, product.Name, product.Description,
                product.PurchasePrice, product.SalePrice, product.QuantityInStock,
                product.CategoryId, product.IsActive))
            .ToArray();
        var salesByDay = saleList
            .GroupBy(sale => DateOnly.FromDateTime(sale.Date))
            .ToDictionary(group => group.Key, group => group.Sum(sale => sale.TotalAmount));

        var totalSales = saleList.Sum(sale => sale.TotalAmount);
        var totalPurchases = purchaseList.Sum(purchase => purchase.TotalAmount);
        var totalExpenses = expenseList.Sum(expense => expense.Amount);

        return new DashboardDto(
            totalSales,
            totalPurchases,
            totalExpenses,
            totalSales - totalPurchases - totalExpenses,
            lowStock,
            salesByDay);
    }
}

#endregion
