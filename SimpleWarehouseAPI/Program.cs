using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<WarehouseService>();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

// API endpoints
app.MapGet("/", () => "🏭 Warehouse Management API - Running Successfully!");

app.MapGet("/api/health", () => new {
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0",
    Message = "Warehouse Management System API is running!"
});

app.MapGet("/api/products", (WarehouseService service) =>
{
    var products = service.GetAllProducts();
    return Results.Ok(new {
        Success = true,
        Count = products.Count,
        Data = products
    });
});

app.MapGet("/api/products/{id:int}", (int id, WarehouseService service) =>
{
    var product = service.GetProductById(id);
    if (product == null)
        return Results.NotFound(new { Success = false, Message = "Product not found" });

    return Results.Ok(new { Success = true, Data = product });
});

app.MapGet("/api/products/search", (string? q, WarehouseService service) =>
{
    if (string.IsNullOrWhiteSpace(q))
        return Results.BadRequest(new { Success = false, Message = "Query parameter 'q' is required" });

    var results = service.SearchProducts(q);
    return Results.Ok(new {
        Success = true,
        Query = q,
        Count = results.Count,
        Data = results
    });
});

app.MapGet("/api/products/low-stock", (WarehouseService service) =>
{
    var lowStockProducts = service.GetLowStockProducts();
    return Results.Ok(new {
        Success = true,
        Count = lowStockProducts.Count,
        Data = lowStockProducts
    });
});

app.MapGet("/api/movements", (WarehouseService service) =>
{
    var movements = service.GetRecentMovements();
    return Results.Ok(new {
        Success = true,
        Count = movements.Count,
        Data = movements
    });
});

Console.WriteLine("🚀 Starting Warehouse Management API...");
Console.WriteLine("📍 API will be available at: http://localhost:5000");
Console.WriteLine("📍 Health check: http://localhost:5000/api/health");
Console.WriteLine("📍 Products: http://localhost:5000/api/products");
Console.WriteLine("📍 Search: http://localhost:5000/api/products/search?q=laptop");
Console.WriteLine("📍 Low Stock: http://localhost:5000/api/products/low-stock");

app.Run();

// Simple models
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string SKU { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int MinStock { get; set; }
    public string Category { get; set; } = "";
}

public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string Type { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTime Date { get; set; }
}

// Simple service
public class WarehouseService
{
    private static List<Product> _products = new();
    private static List<StockMovement> _movements = new();
    private static int _nextId = 1;
    private static int _nextMovementId = 1;

    static WarehouseService()
    {
        InitializeSampleData();
    }

    private static void InitializeSampleData()
    {
        _products.AddRange(new[]
        {
            new Product { Id = _nextId++, Name = "Laptop Dell XPS", SKU = "LAP001", Price = 1299.99m, Stock = 15, MinStock = 5, Category = "Electronics" },
            new Product { Id = _nextId++, Name = "Mouse Wireless", SKU = "MOU001", Price = 29.99m, Stock = 50, MinStock = 10, Category = "Accessories" },
            new Product { Id = _nextId++, Name = "Keyboard Mechanical", SKU = "KEY001", Price = 89.99m, Stock = 3, MinStock = 5, Category = "Accessories" },
            new Product { Id = _nextId++, Name = "Monitor 24 inch", SKU = "MON001", Price = 299.99m, Stock = 8, MinStock = 3, Category = "Electronics" },
            new Product { Id = _nextId++, Name = "USB Cable", SKU = "CAB001", Price = 9.99m, Stock = 100, MinStock = 20, Category = "Cables" }
        });
    }

    public List<Product> GetAllProducts() => _products.ToList();
    public Product? GetProductById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public List<Product> SearchProducts(string query)
    {
        return _products.Where(p =>
            p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.SKU.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            p.Category.Contains(query, StringComparison.OrdinalIgnoreCase)
        ).ToList();
    }

    public List<Product> GetLowStockProducts()
    {
        return _products.Where(p => p.Stock <= p.MinStock).ToList();
    }

    public void AddStock(int productId, int quantity, string reason)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product != null)
        {
            product.Stock += quantity;
            _movements.Add(new StockMovement
            {
                Id = _nextMovementId++,
                ProductId = productId,
                Quantity = quantity,
                Type = "IN",
                Reason = reason,
                Date = DateTime.Now
            });
        }
    }

    public void RemoveStock(int productId, int quantity, string reason)
    {
        var product = _products.FirstOrDefault(p => p.Id == productId);
        if (product != null && product.Stock >= quantity)
        {
            product.Stock -= quantity;
            _movements.Add(new StockMovement
            {
                Id = _nextMovementId++,
                ProductId = productId,
                Quantity = -quantity,
                Type = "OUT",
                Reason = reason,
                Date = DateTime.Now
            });
        }
    }

    public List<StockMovement> GetRecentMovements(int count = 10)
    {
        return _movements.OrderByDescending(m => m.Date).Take(count).ToList();
    }
}


