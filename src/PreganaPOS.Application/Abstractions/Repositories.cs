using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Application.Abstractions;

#region Produits

public interface IProductRepository
{
    int NextId();
    Product? GetById(int id);
    Product? GetByCode(string code);
    IReadOnlyCollection<Product> Search(string? name, string? code, int? categoryId, bool includeInactive);
    void Add(Product product);
    void Update(Product product);
}

#endregion

#region Category

public interface ICategoryRepository
{
    Category? GetById(int id);
    IReadOnlyCollection<Category> GetAll();
    void Add(Category category);
}

#endregion

#region Client

public interface ICustomerRepository
{
    Customer? GetById(int id);
    void Add(Customer customer);
}

#endregion

#region Vente
public interface ISaleRepository
{
    int NextId();
    Sale? GetById(int id);
    IReadOnlyCollection<Sale> GetBetween(DateTime start, DateTime end);
    void Add(Sale sale);
}

#endregion

#region Achat
public interface IPurchaseRepository
{
    IReadOnlyCollection<Purchase> GetBetween(DateTime start, DateTime end);
    void Add(Purchase purchase);
}

#endregion

#region Frais
public interface IExpenseRepository
{
    IReadOnlyCollection<Expense> GetBetween(DateOnly start, DateOnly end);
    void Add(Expense expense);
}
#endregion

#region Unit Of Work

public interface IUnitOfWork
{
    void Execute(Action operation);
}
#endregion
