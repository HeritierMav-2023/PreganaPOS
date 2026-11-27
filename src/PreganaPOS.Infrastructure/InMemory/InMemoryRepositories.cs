using PreganaPOS.Application.Abstractions;
using PreganaPOS.Domain.Entities;

namespace PreganaPOS.Infrastructure.InMemory;

/// <summary>
/// Classe en Memoire 
/// </summary>
public sealed class InMemoryProductRepository : IProductRepository
{
    #region Propriétes
  
    private readonly List<Product> products = [];
    private int nextId = 1;

    #endregion

    #region Methodes Public
   
    public int NextId() => nextId++;
    public Product? GetById(int id) => products.FirstOrDefault(product => product.Id == id);
    public Product? GetByCode(string code) => products.FirstOrDefault(product => string.Equals(product.Code, code, StringComparison.OrdinalIgnoreCase));
    public IReadOnlyCollection<Product> Search(string? name, string? code, int? categoryId, bool includeInactive) => products
        .Where(product => includeInactive || product.IsActive)
        .Where(product => string.IsNullOrWhiteSpace(name) || product.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
        .Where(product => string.IsNullOrWhiteSpace(code) || product.Code.Contains(code, StringComparison.OrdinalIgnoreCase))
        .Where(product => !categoryId.HasValue || product.CategoryId == categoryId.Value)
        .OrderBy(product => product.Name)
        .ToArray();
    public void Add(Product product) => products.Add(product);
    public void Update(Product product) { }

    #endregion
}

#region InMemory Category 

public sealed class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly List<Category> categories = [];
    public Category? GetById(int id) => categories.FirstOrDefault(category => category.Id == id);
    public IReadOnlyCollection<Category> GetAll() => categories.AsReadOnly();
    public void Add(Category category) => categories.Add(category);
}

#endregion

#region InMemory Customer 

public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private readonly List<Customer> customers = [];
    public Customer? GetById(int id) => customers.FirstOrDefault(customer => customer.Id == id);
    public void Add(Customer customer) => customers.Add(customer);
}
#endregion

#region InMemory Sale 

public sealed class InMemorySaleRepository : ISaleRepository
{
    private readonly List<Sale> sales = [];
    private int nextId = 1;
    public int NextId() => nextId++;
    public Sale? GetById(int id) => sales.FirstOrDefault(sale => sale.Id == id);
    public IReadOnlyCollection<Sale> GetBetween(DateTime start, DateTime end) => sales.Where(sale => sale.Date >= start && sale.Date <= end).ToArray();
    public void Add(Sale sale) => sales.Add(sale);
}
#endregion

#region InMemory Purchase 

public sealed class InMemoryPurchaseRepository : IPurchaseRepository
{
    private readonly List<Purchase> purchases = [];
    public IReadOnlyCollection<Purchase> GetBetween(DateTime start, DateTime end) => purchases.Where(purchase => purchase.Date >= start && purchase.Date <= end).ToArray();
    public void Add(Purchase purchase) => purchases.Add(purchase);
}
#endregion

#region InMemory Expense 

public sealed class InMemoryExpenseRepository : IExpenseRepository
{
    private readonly List<Expense> expenses = [];
    public IReadOnlyCollection<Expense> GetBetween(DateOnly start, DateOnly end) => expenses.Where(expense => expense.Date >= start && expense.Date <= end).ToArray();
    public void Add(Expense expense) => expenses.Add(expense);
}
#endregion

#region InMemory Unit Of Work

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    private readonly object syncRoot = new();
    public void Execute(Action operation)
    {
        ArgumentNullException.ThrowIfNull(operation);
        lock (syncRoot)
        {
            operation();
        }
    }
}
#endregion