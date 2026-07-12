using Microsoft.AspNetCore.DataProtection;

namespace comentapp.infrastructure.Service.Implementation
{
    public class TokenProtector : ITokenProtector
    {
        // Purpose dedicado: separa criptográficamente estos tokens de otros usos de DataProtection.
        private const string Purpose = "comentapp.mercadopago.oauth-tokens.v1";

        private readonly IDataProtector _protector;

        public TokenProtector(IDataProtectionProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);
            _protector = provider.CreateProtector(Purpose);
        }

        public string Protect(string plaintext) => _protector.Protect(plaintext);

        public string Unprotect(string ciphertext) => _protector.Unprotect(ciphertext);
    }
}
