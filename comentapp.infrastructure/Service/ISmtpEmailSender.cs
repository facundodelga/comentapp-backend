namespace comentapp.infrastructure.Service
{
    public interface ISmtpEmailSender
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
