using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Sample data
var products = new List<Product>
{
    new Product { Id = 1, Name = "Laptop Dell XPS", SKU = "LAP001", Price = 1299.99m, Stock = 15, MinStock = 5, Category = "Electronics" },
    new Product { Id = 2, Name = "Mouse Wireless", SKU = "MOU001", Price = 29.99m, Stock = 50, MinStock = 10, Category = "Accessories" },
    new Product { Id = 3, Name = "Keyboard Mechanical", SKU = "KEY001", Price = 89.99m, Stock = 3, MinStock = 5, Category = "Accessories" },
    new Product { Id = 4, Name = "Monitor 24 inch", SKU = "MON001", Price = 299.99m, Stock = 8, MinStock = 3, Category = "Electronics" },
    new Product { Id = 5, Name = "USB Cable", SKU = "CAB001", Price = 9.99m, Stock = 100, MinStock = 20, Category = "Cables" }
};

var movements = new List<StockMovement>();

// API Endpoints
app.MapGet("/", () => new {
    message = "🏭 Warehouse Management API",
    version = "1.0.0",
    status = "Running",
    endpoints = new[] {
        "GET /api/health - Health check",
        "GET /api/products - Get all products",
        "GET /api/products/{id} - Get product by ID",
        "GET /api/products/low-stock - Get low stock products",
        "GET /api/movements - Get stock movements"
    }
});

app.MapGet("/api/health", () => new {
    status = "Healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0",
    message = "Warehouse Management System API is running!"
});

app.MapGet("/api/products", () => new {
    success = true,
    count = products.Count,
    data = products
});

app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
        return Results.NotFound(new { success = false, message = "Product not found" });

    return Results.Ok(new { success = true, data = product });
});

app.MapGet("/api/products/low-stock", () =>
{
    var lowStockProducts = products.Where(p => p.Stock <= p.MinStock).ToList();
    return new { success = true, count = lowStockProducts.Count, data = lowStockProducts };
});

app.MapGet("/api/movements", () => new {
    success = true,
    count = movements.Count,
    data = movements
});

Console.WriteLine("🚀 Starting Warehouse Management API...");
Console.WriteLine("📍 API will be available at:");
Console.WriteLine("   - http://localhost:5001");
Console.WriteLine("📊 Swagger UI: http://localhost:5001/swagger");
Console.WriteLine("🔄 Press Ctrl+C to stop");

app.Run("http://localhost:5001");

// Models
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
    public string Type { get; set; } = ""; // IN or OUT
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public string Notes { get; set; } = "";
}
