using Autofac;
using comentapp.infrastructure.Modules;
using comentapp.persistence;
using comentapp.persistence.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace comentapp.authentication.businessLogic
{
    public class AuthenticationBusinessModule(IConfiguration configuration) : Module
    {
        private readonly IConfiguration _configuration = configuration;
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PasswordHasher<User>>()
           .As<IPasswordHasher<User>>()
           .InstancePerLifetimeScope();

            // Escanea todos los servicios del assembly automáticamente
            builder.RegisterAssemblyTypes(ThisAssembly)
                   .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            builder.RegisterModule(new DatabaseModule(_configuration));
            builder.RegisterModule(new EmailModule(_configuration));


        }
    }
    
}
