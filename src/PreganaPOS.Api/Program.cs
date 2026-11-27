using PreganaPOS.Application.Abstractions;
using PreganaPOS.Application.Dashboard;
using PreganaPOS.Application.Products;
using PreganaPOS.Application.Sales;
using PreganaPOS.Infrastructure.InMemory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

#region Injection de Dépendance

builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
builder.Services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
builder.Services.AddSingleton<ISaleRepository, InMemorySaleRepository>();
builder.Services.AddSingleton<IPurchaseRepository, InMemoryPurchaseRepository>();
builder.Services.AddSingleton<IExpenseRepository, InMemoryExpenseRepository>();
builder.Services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
builder.Services.AddSingleton<ProductService>();
builder.Services.AddSingleton<CreateSaleService>();
builder.Services.AddSingleton<DashboardService>();

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

#region Map Post Produit


app.MapPost("/api/products", (CreateProductCommand command, ProductService service) =>
{
    try
    {
        var product = service.Create(command);
        return Results.Created($"/api/products/{product.Id}", product);
    }
    catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
});
#endregion

#region Map Get Produits


app.MapGet("/api/products", (
    string? name,
    string? code,
    int? categoryId,
    bool includeInactive,
    int page,
    int pageSize,
    ProductService service) =>
{
    try
    {
        var query = new ProductSearchQuery(
            name,
            code,
            categoryId,
            includeInactive,
            page == 0 ? 1 : page,
            pageSize == 0 ? 20 : pageSize);
        return Results.Ok(service.Search(query));
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
});
#endregion

#region Map Post Ventes


app.MapPost("/api/sales", (CreateSaleCommand command, CreateSaleService service) =>
{
    try
    {
        var sale = service.Execute(command);
        return Results.Created($"/api/sales/{sale.Id}", sale);
    }
    catch (Exception exception) when (exception is ArgumentException or InvalidOperationException)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
});

#endregion

#region Map Get Dashboard

app.MapGet("/api/dashboard", (DateTime? start, DateTime? end, int? lowStockThreshold, DashboardService service) =>
{
    var from = start ?? DateTime.UtcNow.Date;
    var to = end ?? DateTime.UtcNow;
    try
    {
        return Results.Ok(service.Get(from, to, lowStockThreshold ?? 5));
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { error = exception.Message });
    }
});
#endregion

app.Run();