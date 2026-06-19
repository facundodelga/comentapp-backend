using Autofac;
using comentapp.infrastructure.Options;
using Microsoft.Extensions.Configuration;

namespace comentapp.infrastructure.Modules
{
    public class JwtModule : Module
    {
        private readonly IConfiguration _configuration;

        public JwtModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var jwtOptions = new JwtOptions();
            _configuration.GetSection(JwtOptions.Section).Bind(jwtOptions);

            builder.RegisterInstance(jwtOptions).SingleInstance();
        }
    }
}
