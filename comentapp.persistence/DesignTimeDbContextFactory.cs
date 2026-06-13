using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace comentapp.persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ComentappDbContext>
    {
        public ComentappDbContext CreateDbContext(string[] args)
        {
            // Lee appsettings desde la API para no duplicar config
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../comentapp.authentication.manager"))
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var options = new DbContextOptionsBuilder<ComentappDbContext>()
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .Options;

            return new ComentappDbContext(options);
        }
    }
}
