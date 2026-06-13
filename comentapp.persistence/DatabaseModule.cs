using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace comentapp.persistence
{
    public class DatabaseModule : Module
    {
        private readonly IConfiguration _configuration;

        public DatabaseModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // El módulo se configura solo
            var connectionString = _configuration.GetConnectionString("DefaultConnection")!;

            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            // Si necesitás DbContextOptions dentro del módulo
            builder.Register(_ =>
            {
                var options = new DbContextOptionsBuilder<ComentappDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;
                return options;
            }).SingleInstance();
        }
    }
}
