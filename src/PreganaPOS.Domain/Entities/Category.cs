namespace PreganaPOS.Domain.Entities;

public sealed class Category
{
    private readonly List<Product> products = [];

    public int Id { get; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public IReadOnlyCollection<Product> Products => products.AsReadOnly();

    public Category(int id, string name, string? description = null)
    {
        ValidateName(name);

        Id = id;
        Name = name.Trim();
        Description = description;
    }

    public void Update(string name, string? description)
    {
        ValidateName(name);

        Name = name.Trim();
        Description = description;
    }

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

    public void RemoveProduct(Product product) => products.Remove(product);

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Le nom de la catégorie est obligatoire.", nameof(name));
        }
    }
}
