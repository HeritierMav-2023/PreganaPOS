namespace PreganaPOS.Domain.Entities;

/// <summary>
/// Classe Category
/// </summary>
public sealed class Category
{
    #region Propriétes 

    private readonly List<Product> products = []; // collection de produits associé

    public int Id { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Product> Products => products.AsReadOnly();

    #endregion

    #region Constructeur

    public Category(int id, string name, string? description = null)
    {
        ValidateName(name);

        Id = id;
        Name = name.Trim();
        Description = description;
    }

    #endregion

    #region Methodes

   /// <summary>
   /// Modification de catégories
   /// </summary>
   /// <param name="name"></param>
   /// <param name="description"></param>
    public void Update(string name, string? description)
    {
        ValidateName(name);

        Name = name.Trim();
        Description = description;
    }


    /// <summary>
    /// Ajout d'un produits
    /// </summary>
    /// <param name="product"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddProduct(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (product.CategoryId != Id)
        {
            throw new InvalidOperationException("Le produit n'appartient pas à cette catégorie.");
        }

        if (!products.Contains(product))
        {
            products.Add(product);
        }
    }

    /// <summary>
    /// Suppréssion d'un produit
    /// </summary>
    /// <param name="product"></param>
    public void RemoveProduct(Product product) => products.Remove(product);

    /// <summary>
    /// Validation du nom catégorie
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentException"></exception>
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Le nom de la catégorie est obligatoire.", nameof(name));
        }
    }
    #endregion
}
