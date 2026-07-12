using Autofac;
using comentapp.infrastructure.Options;
using comentapp.infrastructure.Service;
using comentapp.infrastructure.Service.Implementation;
using MercadoPago.Config;
using Microsoft.Extensions.Configuration;

namespace comentapp.infrastructure.Modules
{
    /// <summary>
    /// Registra las opciones de Mercado Pago e inicializa el SDK oficial.
    /// Espejo de <see cref="JwtModule"/> / <see cref="EmailModule"/>.
    /// El access token global es el de la app; los pagos por creador pasan el
    /// token OAuth del creador vía RequestOptions en cada llamada.
    /// </summary>
    public class MercadoPagoModule : Module
    {
        private readonly IConfiguration _configuration;

        public MercadoPagoModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var options = new MercadoPagoOptions();
            _configuration.GetSection(MercadoPagoOptions.Section).Bind(options);

            // Inicializa el SDK (equivalente a MercadoPagoConfig.AccessToken = "...").
            MercadoPagoConfig.AccessToken = options.AccessToken;

            builder.RegisterInstance(options).SingleInstance();

            builder.RegisterType<MercadoPagoPreferenceService>()
                .As<IMercadoPagoPreferenceService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<MercadoPagoOAuthService>()
                .As<IMercadoPagoOAuthService>()
                .InstancePerLifetimeScope();

            builder.RegisterType<TokenProtector>()
                .As<ITokenProtector>()
                .InstancePerLifetimeScope();
        }
    }
}
