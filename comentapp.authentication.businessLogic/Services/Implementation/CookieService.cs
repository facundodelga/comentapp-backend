using comentapp.authentication.businessLogic.Core;
using comentapp.infrastructure.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace comentapp.authentication.businessLogic.Services.Implementation
{
    /// <summary>
    /// Servicio para manejar cookies de autenticación.
    /// Integrado con el esquema "AppCookie" de ASP.NET Core.
    /// Soporta tanto autenticación propia como externa (Google OAuth).
    /// </summary>
    public class CookieService : ICookieService
    {
        private const string AccessTokenCookie = "access_token";
        private const string RefreshTokenCookie = "refresh_token";

        private readonly JwtOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieService(JwtOptions jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _jwtOptions = jwtOptions;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Establecer cookies de autenticación usando el esquema "AppCookie".
        /// Crea un principal con los tokens y lo guarda en la cookie.
        /// </summary>
        public async void SetAuthCookies(HttpResponse response, AuthTokens tokens)
        {
            var httpContext = _httpContextAccessor.HttpContext 
                ?? throw new InvalidOperationException("HttpContext no disponible");

            // Crear claims del usuario
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, tokens.UserId.ToString()),
                new Claim(ClaimTypes.Name, tokens.UserName ?? ""),
                new Claim(ClaimTypes.Email, tokens.Email ?? ""),

                // Tokens JWT en claims (para acceso después)
                new Claim("access_token", tokens.AccessToken),
                new Claim("refresh_token", tokens.RefreshToken),

                // Metadata de autenticación
                new Claim("auth_provider", tokens.AuthProvider ?? "local"),
                new Claim("authenticated_at", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),

                // Expiración absoluta de sesión (24 horas)
                new Claim("absolute_exp_utc", 
                    DateTimeOffset.UtcNow.AddHours(24).ToUnixTimeSeconds().ToString())
            };

            // Crear identity y principal
            var identity = new ClaimsIdentity(claims, "AppCookie");
            var principal = new ClaimsPrincipal(identity);

            // Opciones de autenticación
            var authProperties = new AuthenticationProperties
            {
                // Duración deslizante de la cookie (8 horas)
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
                IsPersistent = true,
                IssuedUtc = DateTimeOffset.UtcNow
            };

            // Firmar en el esquema "AppCookie"
            httpContext.SignInAsync("AppCookie", principal, authProperties).Wait();
        }

        /// <summary>
        /// Limpiar cookies de autenticación.
        /// </summary>
        public async void ClearAuthCookies(HttpResponse response)
        {
            var httpContext = _httpContextAccessor.HttpContext 
                ?? throw new InvalidOperationException("HttpContext no disponible");

            // Firmar fuera del esquema "AppCookie"
            await httpContext.SignOutAsync("AppCookie");

            // También limpiar la cookie externa si existe (para OAuth)
            try
            {
                await httpContext.SignOutAsync("ExternalCookie");
            }
            catch
            {
                // Ignorar si no existe
            }
        }

        /// <summary>
        /// Obtener el token de refresco desde las cookies o claims.
        /// </summary>
        public string? GetRefreshToken(HttpRequest request)
        {
            // Primero, intentar obtener del claim del principal
            var refreshTokenClaim = request.HttpContext.User
                .FindFirst("refresh_token")?.Value;

            if (!string.IsNullOrEmpty(refreshTokenClaim))
                return refreshTokenClaim;

            // Fallback: obtener de la cookie directa (compatibilidad)
            return request.Cookies[RefreshTokenCookie];
        }
    }
}
