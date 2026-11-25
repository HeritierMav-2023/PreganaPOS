namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Achat 
/// </summary>
public sealed class Purchase
{
    #region Propriétes

    private readonly List<PurchaseItem> items = [];

    public int Id { get; }
    public string? SupplierName { get; }
    public DateTime Date { get; }
    public decimal TotalAmount => items.Sum(item => item.Total);
    public IReadOnlyCollection<PurchaseItem> Items => items.AsReadOnly();
    public bool IsValidated { get; private set; }

    #endregion

    #region Constructeurs

    public Purchase(int id, DateTime date, string? supplierName = null)
    {
        Id = id;
        Date = date;
        SupplierName = supplierName;
    }

    #endregion

    #region Methodes

    public void AddItem(int productId, int quantity, decimal unitCost)
    {
        EnsureNotValidated();
        items.Add(new PurchaseItem(productId, quantity, unitCost));
    }

    public void Validate(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        EnsureNotValidated();

        if (items.Count == 0)
        {
            throw new InvalidOperationException("Un achat doit contenir au moins un produit.");
        }

        var productById = products.ToDictionary(product => product.Id);
        foreach (var item in items)
        {
            if (!productById.ContainsKey(item.ProductId))
            {
                throw new InvalidOperationException($"Le produit {item.ProductId} est introuvable.");
            }
        }

        foreach (var item in items)
        {
            productById[item.ProductId].IncreaseStock(item.Quantity);
        }

        IsValidated = true;
    }

    private void EnsureNotValidated()
    {
        if (IsValidated)
        {
            throw new InvalidOperationException("Un achat validé ne peut plus être modifié.");
        }
    }

    #endregion
}
