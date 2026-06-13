using Autofac;
using comentapp.infrastructure.Service;
using comentapp.infrastructure.Service.Implementation;
using Microsoft.Extensions.Configuration;

namespace comentapp.infrastructure.Modules
{
    public class EmailModule : Module
    {
        private readonly IConfiguration _configuration;

        public EmailModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            // Registra las opciones de email
            builder.Register(_ =>
            {
                var options = new EmailOptions();
                _configuration.GetSection(EmailOptions.Section).Bind(options);
                return options;
            }).SingleInstance();

            // Registra los servicios
            builder.RegisterType<SmtpEmailSender>()
                   .As<ISmtpEmailSender>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<EmailTemplateRenderer>()
                   .As<IEmailTemplateRenderer>()
                   .InstancePerLifetimeScope();

            builder.RegisterType<EmailConfirmationService>()
                   .As<IEmailConfirmationService>()
                   .InstancePerLifetimeScope();
        }
    }
}
