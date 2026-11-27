using PreganaPOS.Application.Abstractions;
using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Application.Products;

#region Produit Command

public sealed record CreateProductCommand(
    string Code,
    string Name,
    string? Description,
    decimal PurchasePrice,
    decimal SalePrice,
    int QuantityInStock,
    int CategoryId);

#endregion

#region Produit Query

public sealed record ProductSearchQuery(
    string? Name,
    string? Code,
    int? CategoryId,
    bool IncludeInactive = false,
    int Page = 1,
    int PageSize = 20);

#endregion

#region Produit DTO

public sealed record ProductDto(
    int Id,
    string Code,
    string Name,
    string? Description,
    decimal PurchasePrice,
    decimal SalePrice,
    int QuantityInStock,
    int CategoryId,
    bool IsActive);

#endregion

public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int TotalCount);

#region Produit Service

public sealed class ProductService(IProductRepository products, ICategoryRepository categories)
{
    #region Methode Publique

    public ProductDto Create(CreateProductCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (categories.GetById(command.CategoryId) is null)
        {
            throw new InvalidOperationException("La catégorie est introuvable.");
        }

        if (products.GetByCode(command.Code) is not null)
        {
            throw new InvalidOperationException("Le code produit existe déjà.");
        }

        var product = new Product(
            products.NextId(),
            command.Code,
            command.Name,
            command.Description,
            command.PurchasePrice,
            command.SalePrice,
            command.QuantityInStock,
            command.CategoryId);

        products.Add(product);
        return ToDto(product);
    }

    public PagedResult<ProductDto> Search(ProductSearchQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Page < 1 || query.PageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(query), "La page et la taille doivent être positives.");
        }

        var all = products.Search(query.Name, query.Code, query.CategoryId, query.IncludeInactive);
        var page = all.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).Select(ToDto).ToArray();
        return new PagedResult<ProductDto>(page, query.Page, query.PageSize, all.Count);
    }

    #endregion

    #region Methode private

    private static ProductDto ToDto(Product product) => new(
        product.Id,
        product.Code,
        product.Name,
        product.Description,
        product.PurchasePrice,
        product.SalePrice,
        product.QuantityInStock,
        product.CategoryId,
        product.IsActive);

    #endregion
}

#endregion