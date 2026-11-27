using PreganaPOS.Application.Abstractions;
using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Application.Sales;

#region Vente Command

public sealed record CreateSaleItemCommand(int ProductId, int Quantity, decimal Discount = 0);

public sealed record CreateSaleCommand(
    int? CustomerId,
    PaymentMethod PaymentMethod,
    IReadOnlyCollection<CreateSaleItemCommand> Items);

#endregion

#region Vente DTO

public sealed record SaleDto(
    int Id,
    string SaleNumber,
    DateTime Date,
    int? CustomerId,
    decimal TotalAmount,
    PaymentMethod PaymentMethod,
    SaleStatus Status,
    IReadOnlyCollection<SaleItem> Items);

#endregion

#region Vente Service

public sealed class CreateSaleService(
    ISaleRepository sales,
    IProductRepository products,
    ICustomerRepository customers,
    IUnitOfWork unitOfWork)
{
    public SaleDto Execute(CreateSaleCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (command.Items is null || command.Items.Count == 0)
        {
            throw new InvalidOperationException("Une vente doit contenir au moins un article.");
        }

        if (command.CustomerId.HasValue && customers.GetById(command.CustomerId.Value) is null)
        {
            throw new InvalidOperationException("Le client est introuvable.");
        }

        var productsById = command.Items
            .Select(item => products.GetById(item.ProductId) ?? throw new InvalidOperationException($"Le produit {item.ProductId} est introuvable."))
            .ToDictionary(product => product.Id);

        var sale = new Sale(
            sales.NextId(),
            $"V-{DateTime.UtcNow:yyyyMMddHHmmssfff}",
            DateTime.UtcNow,
            command.CustomerId,
            command.PaymentMethod);

        foreach (var item in command.Items)
        {
            sale.AddItem(productsById[item.ProductId], item.Quantity, item.Discount);
        }

        unitOfWork.Execute(() =>
        {
            sale.Validate(productsById.Values);
            sales.Add(sale);
        });

        return new SaleDto(sale.Id, sale.SaleNumber, sale.Date, sale.CustomerId, sale.TotalAmount, sale.PaymentMethod, sale.Status, sale.Items);
    }
}
#endregion
