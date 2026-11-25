namespace PreganaPOS.Domain.Entities;
/// <summary>
/// Classe Article en Solde
/// </summary>
public sealed class SaleItem
{
    #region Propriétes

    public int ProductId { get; }
    public string ProductName { get; }
    public int Quantity { get; }
    public decimal UnitPrice { get; }
    public decimal Discount { get; }
    public decimal Total => Quantity * UnitPrice - Discount;

    #endregion

    #region Constructeur

    public SaleItem(int productId, string productName, int quantity, decimal unitPrice, decimal discount = 0)
    {
        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new ArgumentException("Le nom du produit est obligatoire.", nameof(productName));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La quantité doit être supérieure à zéro.");
        }

        if (unitPrice < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Le prix unitaire ne peut pas être négatif.");
        }

        if (discount < 0 || discount > quantity * unitPrice)
        {
            throw new ArgumentOutOfRangeException(nameof(discount), "La remise est invalide.");
        }

        ProductId = productId;
        ProductName = productName.Trim();
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
    }
    #endregion
}
