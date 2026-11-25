namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Article Retourné
/// </summary>
public sealed class SaleReturn
{
    #region Propriétes

    private readonly Dictionary<int, int> items = [];

    public int Id { get; }
    public int SaleId { get; }
    public DateTime Date { get; }
    public decimal RefundedAmount { get; private set; }
    public bool IsValidated { get; private set; }
    public IReadOnlyDictionary<int, int> Items => items;

    #endregion

    #region Constructeurs

    public SaleReturn(int id, int saleId, DateTime date)
    {
        Id = id;
        SaleId = saleId;
        Date = date;
    }

    #endregion

    #region Methode Public


    public void AddItem(int productId, int quantity)
    {
        EnsureNotValidated();

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La quantité retournée doit être supérieure à zéro.");
        }

        items[productId] = items.GetValueOrDefault(productId) + quantity;
    }

    public void Validate(Sale sale, IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(sale);
        ArgumentNullException.ThrowIfNull(products);
        EnsureNotValidated();

        if (sale.Status == SaleStatus.Draft)
        {
            throw new InvalidOperationException("Seule une vente validée peut être retournée.");
        }

        if (items.Count == 0)
        {
            throw new InvalidOperationException("Un retour doit contenir au moins un article.");
        }

        var soldItems = sale.Items.GroupBy(item => item.ProductId).ToDictionary(group => group.Key, group => group.ToArray());
        var productById = products.ToDictionary(product => product.Id);
        foreach (var (productId, quantity) in items)
        {
            if (!soldItems.TryGetValue(productId, out var soldLines) || !sale.CanReturn(productId, quantity))
            {
                throw new InvalidOperationException("La quantité retournée dépasse la quantité vendue.");
            }

            if (!productById.ContainsKey(productId))
            {
                throw new InvalidOperationException($"Le produit {productId} est introuvable.");
            }
        }

        foreach (var (productId, quantity) in items)
        {
            sale.RegisterReturn(productId, quantity);
            productById[productId].IncreaseStock(quantity);
            var soldLines = soldItems[productId];
            var unitRefund = soldLines[0].UnitPrice;
            var discountPerUnit = soldLines.Sum(item => item.Discount) / soldLines.Sum(item => item.Quantity);
            RefundedAmount += quantity * (unitRefund - discountPerUnit);
        }

        IsValidated = true;
    }

    #endregion

    #region Methode Private
    private void EnsureNotValidated()
    {
        if (IsValidated)
        {
            throw new InvalidOperationException("Un retour validé ne peut plus être modifié.");
        }
    }
    #endregion
}
