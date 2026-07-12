namespace comentapp.infrastructure.Service
{
    /// <summary>
    /// Resultado del intercambio/refresh de tokens OAuth de Mercado Pago (marketplace).
    /// </summary>
    public class MercadoPagoTokenResult
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
        public required string MpUserId { get; init; }
        public string? PublicKey { get; init; }
        public int ExpiresInSeconds { get; init; }
        public bool LiveMode { get; init; }
    }

    /// <summary>
    /// Adapter del flujo OAuth de Mercado Pago Connect: arma la URL de autorización
    /// y canjea/renueva los tokens del creador (collector). Usa las credenciales de la
    /// app (ClientId/ClientSecret), no el access token de ningún creador.
    /// </summary>
    public interface IMercadoPagoOAuthService
    {
        /// <summary>URL a la que redirigir al creador para autorizar la conexión. <paramref name="state"/> es anti-CSRF.</summary>
        string BuildAuthorizationUrl(string state);

        /// <summary>Canjea el <paramref name="code"/> del callback por access/refresh token del creador.</summary>
        Task<MercadoPagoTokenResult> ExchangeCodeAsync(string code);

        /// <summary>Renueva el access token usando el refresh token del creador.</summary>
        Task<MercadoPagoTokenResult> RefreshAsync(string refreshToken);
    }
}
