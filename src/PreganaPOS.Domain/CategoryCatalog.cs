using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Domain;

public sealed class CategoryCatalog
{
    private readonly List<Category> categories = [];

    public IReadOnlyCollection<Category> Categories => categories.AsReadOnly();

    public void Add(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);
        EnsureNameIsUnique(category.Name, category);
        categories.Add(category);
    }

    public void Update(Category category, string name, string? description)
    {
        ArgumentNullException.ThrowIfNull(category);

        if (!categories.Contains(category))
        {
            throw new InvalidOperationException("La catégorie n'appartient pas à ce catalogue.");
        }

        EnsureNameIsUnique(name, category);
        category.Update(name, description);
    }

    public bool Remove(Category category) => categories.Remove(category);

    private void EnsureNameIsUnique(string name, Category category)
    {
        if (categories.Any(existing =>
                !ReferenceEquals(existing, category) &&
                name is not null &&
                string.Equals(existing.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("Une catégorie portant ce nom existe déjà.");
        }
    }
}
