using System.Net.Http.Json;
using System.Text.Json.Serialization;
using comentapp.infrastructure.Options;

namespace comentapp.infrastructure.Service.Implementation
{
    /// <summary>
    /// Implementa el flujo OAuth de Mercado Pago Connect vía HTTP directo
    /// (el SDK oficial no expone cliente OAuth). Endpoints:
    /// autorización en auth.mercadopago.com.ar, token en api.mercadopago.com/oauth/token.
    /// </summary>
    public class MercadoPagoOAuthService : IMercadoPagoOAuthService
    {
        private const string AuthorizationBaseUrl = "https://auth.mercadopago.com.ar/authorization";
        private const string TokenUrl = "https://api.mercadopago.com/oauth/token";

        private static readonly HttpClient _http = new();
        private readonly MercadoPagoOptions _options;

        public MercadoPagoOAuthService(MercadoPagoOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public string BuildAuthorizationUrl(string state)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(state);

            var query = new Dictionary<string, string>
            {
                ["client_id"] = _options.ClientId,
                ["response_type"] = "code",
                ["platform_id"] = "mp",
                ["state"] = state,
                ["redirect_uri"] = _options.OAuthRedirectUri,
            };

            var qs = string.Join("&", query.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));

            return $"{AuthorizationBaseUrl}?{qs}";
        }

        public Task<MercadoPagoTokenResult> ExchangeCodeAsync(string code)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);

            return PostTokenAsync(new
            {
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                grant_type = "authorization_code",
                code,
                redirect_uri = _options.OAuthRedirectUri,
            });
        }

        public Task<MercadoPagoTokenResult> RefreshAsync(string refreshToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

            return PostTokenAsync(new
            {
                client_id = _options.ClientId,
                client_secret = _options.ClientSecret,
                grant_type = "refresh_token",
                refresh_token = refreshToken,
            });
        }

        private async Task<MercadoPagoTokenResult> PostTokenAsync(object body)
        {
            using var response = await _http.PostAsJsonAsync(TokenUrl, body);
            response.EnsureSuccessStatusCode();

            var dto = await response.Content.ReadFromJsonAsync<OAuthTokenResponse>()
                ?? throw new InvalidOperationException("Respuesta OAuth de Mercado Pago vacía.");

            return new MercadoPagoTokenResult
            {
                AccessToken = dto.AccessToken ?? string.Empty,
                RefreshToken = dto.RefreshToken ?? string.Empty,
                MpUserId = dto.UserId?.ToString() ?? string.Empty,
                PublicKey = dto.PublicKey,
                ExpiresInSeconds = dto.ExpiresIn,
                LiveMode = dto.LiveMode,
            };
        }

        private sealed class OAuthTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string? AccessToken { get; set; }

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            [JsonPropertyName("user_id")]
            public long? UserId { get; set; }

            [JsonPropertyName("public_key")]
            public string? PublicKey { get; set; }

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("live_mode")]
            public bool LiveMode { get; set; }
        }
    }
}
