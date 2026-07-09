using Autofac;
using Autofac.Extensions.DependencyInjection;
using comentapp.authentication.businessLogic;
using comentapp.persistence;
using Comentapp.AuthenticationManager.Endpoint.Mapper;
using Comentapp.AuthenticationManager.Endpoint.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// ========== Configuración Base ==========
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();  // Para acceso a HttpContext en servicios

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ========== Autofac - IoC Container ==========
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
{
    containerBuilder.RegisterModule(new AuthenticationBusinessModule(context.Configuration));
});

// ========== Data Protection ==========
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ComentappDbContext>();

// ========== AutoMapper ==========
builder.Services.AddScoped<AppCookieEvents>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AuthenticationMapperProfile>();
});

// ========== Autenticación Multi-Esquema ==========
builder.Services.AddAuthentication(options =>
{
    // Esquema por defecto para cookies
    options.DefaultScheme = "AppCookie";
    options.DefaultChallengeScheme = "AppCookie";
    options.DefaultSignInScheme = "AppCookie";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Google:ClientId"] ?? string.Empty;
    options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? string.Empty;
    options.SignInScheme = "AppCookie";
    options.Scope.Add("profile");
    options.Scope.Add("email");
})
// Cookie local para autenticación propia (email/contraseña)
.AddCookie("AppCookie", options =>
{
    options.Cookie.Name = "__Host-app_session";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.Path = "/";

    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);  // 8 horas con deslizamiento

    options.EventsType = typeof(AppCookieEvents);

    // Manejo de redirecciones personalizadas
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx =>
        {
            // Para APIs, retornar 401 en lugar de redirigir
            if (ctx.Request.Path.StartsWithSegments("/me") || 
                ctx.Request.Path.StartsWithSegments("/session"))
            {
                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }

            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = ctx =>
        {
            // Para APIs, retornar 403
            if (ctx.Request.Path.StartsWithSegments("/me") || 
                ctx.Request.Path.StartsWithSegments("/session"))
            {
                ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }

            ctx.Response.Redirect(ctx.RedirectUri);
            return Task.CompletedTask;
        },
        OnValidatePrincipal = ctx =>
        {
            // Llamar al validador personalizado
            return ctx.HttpContext.RequestServices
                .GetRequiredService<AppCookieEvents>()
                .ValidatePrincipal(ctx);
        }
    };
})
// Cookie para autenticación externa (Google OAuth, etc.)
.AddCookie("ExternalCookie", options =>
{
    options.Cookie.Name = "__Host-external_session";
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.Path = "/";

    // Más corta para cookies externas
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
});

// ========== Servicios Adicionales ==========
builder.Services.AddLogging();

// ========== CORS (si es necesario) ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(builder.Configuration["Cors:AllowedOrigins"] ?? "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Importante para cookies
    });
});

// ========== Build App ==========
var app = builder.Build();

// ========== Middleware Pipeline ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Orden importante del middleware
app.UseHttpsRedirection();
app.UseCors("AllowFrontend");  // Si se usa CORS
app.UseRouting();

// ¡IMPORTANTE! Autenticación y Autorización
app.UseAuthentication();    // Procesar esquemas de autenticación
app.UseAuthorization();     // Aplicar políticas de autorización

app.MapControllers();
app.Run();
