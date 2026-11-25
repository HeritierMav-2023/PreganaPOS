namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Product
/// </summary>
public sealed class Product
{
    #region Propriétes

    public int Id { get; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public decimal SalePrice { get; private set; }
    public int QuantityInStock { get; private set; }
    public int CategoryId { get; private set; }
    public bool IsActive { get; private set; }

    #endregion

    #region Constructeur

    public Product(
        int id,
        string code,
        string name,
        string? description,
        decimal purchasePrice,
        decimal salePrice,
        int quantityInStock,
        int categoryId,
        bool isActive = true)
    {
        Validate(name, salePrice, quantityInStock);

        Id = id;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name;
        Description = description;
        PurchasePrice = purchasePrice;
        SalePrice = salePrice;
        QuantityInStock = quantityInStock;
        CategoryId = categoryId;
        IsActive = isActive;
    }

    #endregion

    #region Methodes

  
    /// <summary>
    /// Methode update product
    /// </summary>
    /// <param name="code"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="purchasePrice"></param>
    /// <param name="salePrice"></param>
    /// <param name="quantityInStock"></param>
    /// <param name="categoryId"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public void Update(
        string code,
        string name,
        string? description,
        decimal purchasePrice,
        decimal salePrice,
        int quantityInStock,
        int categoryId)
    {
        Validate(name, salePrice, quantityInStock);

        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name;
        Description = description;
        PurchasePrice = purchasePrice;
        SalePrice = salePrice;
        QuantityInStock = quantityInStock;
        CategoryId = categoryId;
    }

    //Methode Activer produit
    public void Activate() => IsActive = true;

    //Methode DesActiver produit
    public void Deactivate() => IsActive = false;

    //Méthode Incrementer Stock
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La quantité doit être supérieure à zéro.");
        }

        QuantityInStock += quantity;
    }


    //Méthode Dencrementer Stock
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La quantité doit être supérieure à zéro.");
        }

        if (quantity > QuantityInStock)
        {
            throw new InvalidOperationException("La quantité demandée dépasse le stock disponible.");
        }

        QuantityInStock -= quantity;
    }

    //Methode Validation  product
    private static void Validate(string name, decimal salePrice, int quantityInStock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Le nom du produit est obligatoire.", nameof(name));
        }

        if (salePrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(salePrice), "Le prix de vente ne peut pas être négatif.");
        }

        if (quantityInStock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityInStock), "Le stock ne peut pas être négatif.");
        }
    }
    #endregion
}
