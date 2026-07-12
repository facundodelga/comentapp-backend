using Autofac;
using Autofac.Extensions.DependencyInjection;
using comentapp.business.endpoint.Services;
using comentapp.infrastructure.Modules;
using comentapp.persistence;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;

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
    containerBuilder.RegisterModule(new DatabaseModule(context.Configuration));
    containerBuilder.RegisterModule(new MercadoPagoModule(context.Configuration));
    //containerBuilder.RegisterModule(new AuthenticationBusinessModule(context.Configuration));

    containerBuilder.RegisterType<MercadoPagoConnectService>()
        .As<IMercadoPagoConnectService>()
        .InstancePerLifetimeScope();

    containerBuilder.RegisterType<CreatorService>()
        .As<ICreatorService>()
        .InstancePerLifetimeScope();

    containerBuilder.RegisterType<DonationCheckoutService>()
        .As<IDonationCheckoutService>()
        .InstancePerLifetimeScope();
});

// ========== Data Protection ==========
// MISMO ApplicationName que comentapp.authentication.manager para poder desencriptar
// la cookie de sesión (__Host-app_session) emitida por esa API. Las claves se comparten
// vía DB (PersistKeysToDbContext).
builder.Services.AddDataProtection()
    .SetApplicationName("ComentApp")
    .PersistKeysToDbContext<ComentappDbContext>();


builder.Services.AddAutoMapper(cfg =>
{
    //cfg.AddProfile<AuthenticationMapperProfile>();
});

// ========== Autenticación ==========
// Valida la cookie de sesión (__Host-app_session) emitida por comentapp.authentication.manager.
// Comparte DataProtection (mismo ApplicationName + claves en DB) para poder desencriptarla.
// No re-emite sesiones ni corre AppCookieEvents: esta API solo consume la sesión existente.
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "AppCookie";
    options.DefaultChallengeScheme = "AppCookie";
})
.AddCookie("AppCookie", options =>
{
    options.Cookie.Name = "__Host-app_session";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.Path = "/";

    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);

    // APIs: 401/403 en lugar de redirigir al login.
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        },
        OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

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
