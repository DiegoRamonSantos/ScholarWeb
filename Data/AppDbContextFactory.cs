using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ScholarWeb.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlite(DatabasePath.ResolveConnectionString(
            Directory.GetCurrentDirectory(),
            DatabasePath.DefaultConnectionString));

        return new AppDbContext(optionsBuilder.Options);
    }
}
