using Microsoft.EntityFrameworkCore;
using WarehouseApp.Core.Interfaces;
using WarehouseApp.Core.Services;
using WarehouseApp.Data.Context;
using WarehouseApp.Data.Interfaces;
using WarehouseApp.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add DbContext with in-memory database for demo
builder.Services.AddDbContext<WarehouseDbContext>(options =>
    options.UseInMemoryDatabase("WarehouseDemo"));

// Add repositories (commented out for now)
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
// builder.Services.AddScoped<IStockRepository, StockRepository>();
// builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStockService, StockService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.UseRouting();

// Add simple API endpoints
app.MapGet("/", () => "🏭 Warehouse Management API - Running Successfully!");

app.MapGet("/api/health", () => new {
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
});

app.MapGet("/api/products", async (IProductService productService) =>
{
    try
    {
        var response = await productService.GetAllAsync();
        if (response.Success)
        {
            return Results.Ok(new {
                Success = true,
                Count = response.Data?.Data?.Count() ?? 0,
                Data = response.Data
            });
        }
        else
        {
            return Results.BadRequest(new {
                Success = false,
                Message = response.Message
            });
        }
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error: {ex.Message}");
    }
});

app.MapControllers();

Console.WriteLine("🚀 Starting Warehouse Management API...");
Console.WriteLine("📍 API will be available at: http://localhost:5000");
Console.WriteLine("📍 Health check: http://localhost:5000/api/health");
Console.WriteLine("📍 Products: http://localhost:5000/api/products");

app.Run();
