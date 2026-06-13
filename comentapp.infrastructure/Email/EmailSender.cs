using comentapp_authentication_manager.Security;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;


namespace comentapp.infrastructure.Email
{
    public class EmailSender(ILogger<EmailSender> logger, IOptions<EmailOptions> options) : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger = logger;
        private readonly EmailOptions _options = options.Value;

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Html) { Text = body };

            using var client = new SmtpClient();

            await client.ConnectAsync(
                _options.Host,
                _options.Port,
                _options.UseSsl
                    ? SecureSocketOptions.SslOnConnect
                    : _options.UseStartTls
                        ? SecureSocketOptions.StartTls
                        : SecureSocketOptions.None
            );

            //await client.AuthenticateAsync(_options.Username, _options.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(quit: true);

            _logger.LogInformation("Email enviado a {To} — asunto: {Subject}", to, subject);
        }
    }
}
