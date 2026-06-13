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
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("No se encontró la connection string 'DefaultConnection'.");
            }

            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.Name.EndsWith("Repository"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            builder.Register(_ =>
            {
                return new DbContextOptionsBuilder<ComentappDbContext>()
                    .UseSqlServer(connectionString)
                    .Options;
            })
            .As<DbContextOptions<ComentappDbContext>>()
            .SingleInstance();

            builder.RegisterType<ComentappDbContext>()
                   .AsSelf()
                   .InstancePerLifetimeScope();
        }
    }
}