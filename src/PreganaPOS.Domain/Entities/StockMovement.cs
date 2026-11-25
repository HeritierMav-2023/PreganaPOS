namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Gerer Stock
/// </summary>
public sealed class StockMovement
{
    #region Propriétes

    public int Id { get; }
    public int ProductId { get; }
    public int Quantity { get; }
    public StockMovementType Type { get; }
    public DateTime Date { get; }
    public string Reason { get; }

    #endregion

    #region Constructeurs

    public StockMovement(
        int id,
        int productId,
        int quantity,
        StockMovementType type,
        DateTime date,
        string reason)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "La quantité doit être supérieure à zéro.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("La raison est obligatoire.", nameof(reason));
        }

        Id = id;
        ProductId = productId;
        Quantity = quantity;
        Type = type;
        Date = date;
        Reason = reason.Trim();
    }

    #endregion
}

#region Enumération

public enum StockMovementType
{
    Entry,
    Exit
}
#endregion