using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Domain;

/// <summary>
/// Classe Inventaire
/// </summary>
public sealed class Inventory
{
    #region Propriéte

    private readonly List<StockMovement> movements = [];

    public IReadOnlyCollection<StockMovement> Movements => movements.AsReadOnly();

    #endregion

    #region Methode Public

    public StockMovement RecordEntry(
        int id,
        Product product,
        int quantity,
        DateTime date,
        string reason)
    {
        ArgumentNullException.ThrowIfNull(product);

        var movement = new StockMovement(id, product.Id, quantity, StockMovementType.Entry, date, reason);
        product.IncreaseStock(quantity);
        movements.Add(movement);
        return movement;
    }

    public StockMovement RecordExit(
        int id,
        Product product,
        int quantity,
        DateTime date,
        string reason)
    {
        ArgumentNullException.ThrowIfNull(product);

        var movement = new StockMovement(id, product.Id, quantity, StockMovementType.Exit, date, reason);
        product.DecreaseStock(quantity);
        movements.Add(movement);
        return movement;
    }
    #endregion
}
