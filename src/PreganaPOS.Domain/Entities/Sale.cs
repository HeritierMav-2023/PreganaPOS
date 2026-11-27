namespace PreganaPOS.Domain.Entities;

#region Enumération

public enum PaymentMethod
{
    Cash,
    Card,
    MobileMoney
}

public enum SaleStatus
{
    Draft,
    Validated,
    Returned
}
#endregion

/// <summary>
/// Classe Vente
/// </summary>
public sealed class Sale
{
    #region Propriétes

    private readonly List<SaleItem> items = [];
    private readonly Dictionary<int, int> returnedQuantities = [];

    public int Id { get; }
    public string SaleNumber { get; }
    public DateTime Date { get; }
    public int? CustomerId { get; }
    public decimal TotalAmount => items.Sum(item => item.Total);
    public PaymentMethod PaymentMethod { get; }
    public SaleStatus Status { get; private set; }
    public IReadOnlyCollection<SaleItem> Items => items.AsReadOnly();

    #endregion

    #region Methodes

    public Sale(
        int id,
        string saleNumber,
        DateTime date,
        int? customerId,
        PaymentMethod paymentMethod)
    {
        if (string.IsNullOrWhiteSpace(saleNumber))
        {
            throw new ArgumentException("Le numéro de vente est obligatoire.", nameof(saleNumber));
        }

        Id = id;
        SaleNumber = saleNumber.Trim();
        Date = date;
        CustomerId = customerId;
        PaymentMethod = paymentMethod;
        Status = SaleStatus.Draft;
    }

    #endregion

    #region Methodes Public

    /// <summary>
    /// Ajouter élement
    /// </summary>
    /// <param name="product"></param>
    /// <param name="quantity"></param>
    /// <param name="discount"></param>
    public void AddItem(Product product, int quantity, decimal discount = 0)
    {
        ArgumentNullException.ThrowIfNull(product);
        EnsureDraft();

        items.Add(new SaleItem(product.Id, product.Name, quantity, product.SalePrice, discount));
    }

    
    public void AddItem(SaleItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        EnsureDraft();
        items.Add(item);
    }

    public void Validate(IEnumerable<Product> products)
    {
        ArgumentNullException.ThrowIfNull(products);
        EnsureDraft();

        if (items.Count == 0)
        {
            throw new InvalidOperationException("Une vente doit contenir au moins un article.");
        }

        var productById = products.ToDictionary(product => product.Id);
        foreach (var item in items.GroupBy(item => item.ProductId).Select(group => new { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) }))
        {
            if (!productById.TryGetValue(item.ProductId, out var product))
            {
                throw new InvalidOperationException($"Le produit {item.ProductId} est introuvable.");
            }

            if (product.QuantityInStock < item.Quantity)
            {
                throw new InvalidOperationException("La quantité demandée dépasse le stock disponible.");
            }
        }

        foreach (var item in items.GroupBy(item => item.ProductId).Select(group => new { ProductId = group.Key, Quantity = group.Sum(item => item.Quantity) }))
        {
            productById[item.ProductId].DecreaseStock(item.Quantity);
        }

        Status = SaleStatus.Validated;
    }

    #endregion

    #region Methode Internal

    internal bool CanReturn(int productId, int quantity)
    {
        var soldQuantity = items.Where(item => item.ProductId == productId).Sum(item => item.Quantity);
        return returnedQuantities.GetValueOrDefault(productId) + quantity <= soldQuantity;
    }

    internal void RegisterReturn(int productId, int quantity)
    {
        if (!CanReturn(productId, quantity))
        {
            throw new InvalidOperationException("La quantité retournée dépasse la quantité vendue.");
        }

        returnedQuantities[productId] = returnedQuantities.GetValueOrDefault(productId) + quantity;
        Status = SaleStatus.Returned;
    }

    #endregion

    #region Methode Private

    private void EnsureDraft()
    {
        if (Status != SaleStatus.Draft)
        {
            throw new InvalidOperationException("Une vente validée ne peut plus être modifiée.");
        }
    }
    #endregion

}
