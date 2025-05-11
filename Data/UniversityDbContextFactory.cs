using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using UniversityAffairs.Data;

public class UniversityDbContextFactory : IDesignTimeDbContextFactory<UniversityDbContext>
{
    public UniversityDbContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory(); // .csproj klasörü
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("UniversityDbContext"));

        return new UniversityDbContext(optionsBuilder.Options);
    }
}
