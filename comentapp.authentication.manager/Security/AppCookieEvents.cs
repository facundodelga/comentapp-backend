using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Comentapp.AuthenticationManager.Endpoint.Security
{
    /// <summary>
    /// Manejador de eventos para las cookies de autenticación "AppCookie".
    /// Responsable de:
    /// - Validar la cookie en cada solicitud
    /// - Verificar la expiración absoluta
    /// - Manejar redirecciones personalizadas
    /// - Registrar eventos de autenticación
    /// </summary>
    public sealed class AppCookieEvents : CookieAuthenticationEvents
    {
        private readonly ILogger<AppCookieEvents> _logger;

        public AppCookieEvents(ILogger<AppCookieEvents> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Validar el principal (usuario) en cada solicitud.
        /// Verifica expiración absoluta y otras validaciones personalizadas.
        /// </summary>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            _logger.LogDebug("[COOKIE VALIDATE] Principal: {Principal}", context.Principal?.Identity?.Name);

            var absoluteExpClaim = context.Principal?.FindFirst("absolute_exp_utc")?.Value;

            // Si no hay claim de expiración absoluta, es válido
            if (string.IsNullOrEmpty(absoluteExpClaim))
            {
                await base.ValidatePrincipal(context);
                return;
            }

            // Parsear la expiración absoluta
            if (!long.TryParse(absoluteExpClaim, out var absoluteExpUnix))
            {
                _logger.LogWarning("[COOKIE VALIDATE] Invalid absolute_exp_utc claim: {Claim}", absoluteExpClaim);
                await RejectAsync(context);
                return;
            }

            // Verificar si ha expirado la sesión absoluta
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now >= absoluteExpUnix)
            {
                _logger.LogInformation("[COOKIE VALIDATE] Session expired. User: {User}", context.Principal?.Identity?.Name);
                await RejectAsync(context);
                return;
            }

            _logger.LogDebug("[COOKIE VALIDATE] Session valid. Remaining: {Remaining}s", absoluteExpUnix - now);
            await base.ValidatePrincipal(context);
        }

        /// <summary>
        /// Manejar redirección a login.
        /// Para APIs, retorna 401. Para web, redirige.
        /// </summary>
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            _logger.LogInformation(
                "[COOKIE REDIRECT TO LOGIN] Path={Path}, RedirectUri={RedirectUri}",
                context.Request.Path,
                context.RedirectUri
            );

            // Para endpoints API, retornar 401 en lugar de redirigir
            if (context.Request.Path.StartsWithSegments("/api") || 
                context.Request.Path.StartsWithSegments("/session") ||
                context.Request.Path.StartsWithSegments("/auth"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Manejar redirección a acceso denegado.
        /// Retorna 403 Forbidden para todas las solicitudes.
        /// </summary>
        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            _logger.LogWarning(
                "[COOKIE ACCESS DENIED] Path={Path}",
                context.Request.Path
            );

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Evento cuando se crea un ticket (login exitoso).
        /// Agrega claims adicionales como expiración absoluta.
        /// </summary>
        public override Task SigningIn(CookieSigningInContext context)
        {
            _logger.LogInformation(
                "[COOKIE SIGNING IN] User: {User}",
                context.Principal?.Identity?.Name
            );

            // Agregar expiración absoluta (diferente del deslizamiento)
            var existingClaims = context.Principal?.Claims.ToList() ?? new();

            // Verificar si ya existe el claim
            if (!existingClaims.Any(c => c.Type == "absolute_exp_utc"))
            {
                var absoluteExp = DateTimeOffset.UtcNow.AddHours(24);  // 24 horas de sesión máxima
                var absoluteExpUnix = absoluteExp.ToUnixTimeSeconds();

                existingClaims.Add(new Claim("absolute_exp_utc", absoluteExpUnix.ToString()));

                if (context.Principal != null)
                {
                    context.Principal = new ClaimsPrincipal(
                        new ClaimsIdentity(existingClaims, context.Principal.Identity?.AuthenticationType)
                    );
                }
            }

            return base.SigningIn(context);
        }

        /// <summary>
        /// Evento cuando se firma la cookie (después de SigningIn).
        /// </summary>
        public override Task SignedIn(CookieSignedInContext context)
        {
            _logger.LogInformation(
                "[COOKIE SIGNED IN] User: {User}",
                context.Principal?.Identity?.Name
            );

            return base.SignedIn(context);
        }

        /// <summary>
        /// Evento cuando se revoca la autenticación.
        /// </summary>
        public override Task SigningOut(CookieSigningOutContext context)
        {
            _logger.LogInformation("[COOKIE SIGNING OUT]");
            return base.SigningOut(context);
        }

        /// <summary>
        /// Rechazar la cookie (logout/expiración).
        /// </summary>
        private async Task RejectAsync(CookieValidatePrincipalContext context)
        {
            _logger.LogWarning("[COOKIE REJECT] Rejecting principal for user: {User}", context.Principal?.Identity?.Name);
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync("AppCookie");
        }
    }
}
