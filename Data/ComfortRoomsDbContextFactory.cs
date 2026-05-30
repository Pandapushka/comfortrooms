using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ComfortRooms.Data;

public sealed class ComfortRoomsDbContextFactory : IDesignTimeDbContextFactory<ComfortRoomsDbContext>
{
    public ComfortRoomsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ComfortRoomsDbContext>();
        optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));

        return new ComfortRoomsDbContext(optionsBuilder.Options);
    }
}
