using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Comentapp.AuthenticationManager.Endpoint.Security
{
    public sealed class AppCookieEvents : CookieAuthenticationEvents
    {
        private readonly ILogger<AppCookieEvents> _logger;
        public AppCookieEvents(ILogger<AppCookieEvents> logger)
        {
            _logger = logger;
        }
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var absoluteExpClaim = context.Principal?.FindFirst("absolute_exp_utc")?.Value;

            if (!long.TryParse(absoluteExpClaim, out var absoluteExpUnix))
            {
                await RejectAsync(context);
                return;
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (now >= absoluteExpUnix)
            {
                await RejectAsync(context);
                return;
            }

            await base.ValidatePrincipal(context);
        }
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            _logger.LogInformation(
                "[COOKIE REDIRECT TO LOGIN] PathBase={PathBase}, Path={Path}, RedirectUri={RedirectUri}",
                context.Request.PathBase,
                context.Request.Path,
                context.RedirectUri
            );

            if (context.Request.Path.StartsWithSegments("/session"))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }

        public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
        {
            _logger.LogInformation(
                "[COOKIE ACCESS DENIED] PathBase={PathBase}, Path={Path}",
                context.Request.PathBase,
                context.Request.Path
            );

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        private static async Task RejectAsync(CookieValidatePrincipalContext context)
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync("Cookies");
        }
    }
}
