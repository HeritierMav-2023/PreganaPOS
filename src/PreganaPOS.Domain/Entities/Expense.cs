namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Frais
/// </summary>
public sealed class Expense
{
    #region Propriétes

    public int Id { get; }
    public string Description { get; private set; }
    public decimal Amount { get; private set; }
    public DateOnly Date { get; private set; }
    public string? Category { get; private set; }

    #endregion

    #region Constructeurs

    public Expense(int id, string description, decimal amount, DateOnly date, string? category = null)
    {
        Validate(description, amount);

        Id = id;
        Description = description.Trim();
        Amount = amount;
        Date = date;
        Category = category;
    }

    #endregion

    #region Methode Public

    public void Update(string description, decimal amount, DateOnly date, string? category = null)
    {
        Validate(description, amount);

        Description = description.Trim();
        Amount = amount;
        Date = date;
        Category = category;
    }

    public static IEnumerable<Expense> FilterByPeriod(
        IEnumerable<Expense> expenses,
        DateOnly startDate,
        DateOnly endDate)
    {
        ArgumentNullException.ThrowIfNull(expenses);

        if (startDate > endDate)
        {
            throw new ArgumentException("La date de début doit précéder la date de fin.");
        }

        return expenses.Where(expense => expense.Date >= startDate && expense.Date <= endDate).ToArray();
    }

    #endregion

    #region Methode Private

    private static void Validate(string description, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("La description est obligatoire.", nameof(description));
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Le montant doit être supérieur à zéro.");
        }
    }
    #endregion
}
