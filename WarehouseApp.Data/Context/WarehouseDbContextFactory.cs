using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WarehouseApp.Data.Context;

public class WarehouseDbContextFactory : IDesignTimeDbContextFactory<WarehouseDbContext>
{
    public WarehouseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();

        // Use SQLite for design-time
        optionsBuilder.UseSqlite("Data Source=warehouse.db");

        return new WarehouseDbContext(optionsBuilder.Options);
    }
}