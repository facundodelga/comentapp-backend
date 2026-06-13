using Microsoft.AspNetCore.DataProtection;

namespace comentapp.infrastructure.Service.Implementation
{
    public class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly ITimeLimitedDataProtector _protector;

        public EmailConfirmationService(IDataProtectionProvider provider)
        {
            _protector = provider
                .CreateProtector("EmailConfirmation")
                .ToTimeLimitedDataProtector();
        }

        public string GenerateToken(int userId, string email)
        {
            var payload = $"{userId}|{email.ToLower()}";

            return _protector.Protect(
                payload,
                lifetime: TimeSpan.FromHours(24)
            );
        }

        public int? ValidateToken(string token, string email)
        {
            try
            {
                
                var payload = _protector.Unprotect(token);

                var parts = payload.Split('|');

                if (parts.Length != 2)
                    return null;

                var userId = parts[0];
                var tokenEmail = parts[1];

                if (tokenEmail != email.ToLower())
                    return null;

                return int.Parse(userId);
            }
            catch
            {
                return null;
            }
        }
    }
}
