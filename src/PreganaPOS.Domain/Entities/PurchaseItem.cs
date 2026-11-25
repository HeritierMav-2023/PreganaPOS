namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Article Acheter
/// </summary>
public sealed class PurchaseItem
{
    #region Propriétes

    public int ProductId { get; }
    public int Quantity { get; }
    public decimal UnitCost { get; }
    public decimal Total => Quantity * UnitCost;

    #endregion

    #region Constructeur
    
    public PurchaseItem(int productId, int quantity, decimal unitCost)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La quantité achetée doit être supérieure à zéro.");
        }

        if (unitCost < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(unitCost), "Le coût unitaire ne peut pas être négatif.");
        }

        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
    }
    #endregion
}
