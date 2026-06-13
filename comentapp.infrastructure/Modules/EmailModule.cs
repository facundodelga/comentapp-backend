using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.infrastructure.Modules
{
    public class EmailModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SmtpEmailSender>()
                   .As<IEmailSender>()
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
