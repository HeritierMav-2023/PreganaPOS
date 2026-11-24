namespace PreganaPOS.Domain.Entities;

public sealed class Product
{
    public int Id { get; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public decimal PurchasePrice { get; private set; }
    public decimal SalePrice { get; private set; }
    public int QuantityInStock { get; private set; }
    public int CategoryId { get; private set; }
    public bool IsActive { get; private set; }

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

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

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
}
