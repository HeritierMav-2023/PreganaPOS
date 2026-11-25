namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Client
/// </summary>
public sealed class Customer
{
    #region Propriétes

    public int Id { get; }
    public string FullName { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }

    #endregion

    #region Constructeurs

    public Customer(
        int id,
        string fullName,
        string? phoneNumber = null,
        string? address = null,
        string? email = null,
        bool isActive = true)
    {
        ValidateName(fullName);

        Id = id;
        FullName = fullName.Trim();
        PhoneNumber = phoneNumber;
        Address = address;
        Email = email;
        IsActive = isActive;
    }

    #endregion

    #region Methodes

    /// <summary>
    /// Modification client
    /// </summary>
    /// <param name="fullName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="address"></param>
    /// <param name="email"></param>
    public void Update(string fullName, string? phoneNumber, string? address, string? email)
    {
        ValidateName(fullName);

        FullName = fullName.Trim();
        PhoneNumber = phoneNumber;
        Address = address;
        Email = email;
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;

    private static void ValidateName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            throw new ArgumentException("Le nom du client est obligatoire.", nameof(fullName));
        }
    }
    #endregion
}
