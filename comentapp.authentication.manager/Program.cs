using Comentapp.AuthenticationManager.Endpoint.Security;
using Comentapp.AuthenticationManager.Endpoint.Services;
using Comentapp.AuthenticationManager.Endpoint.Services.Implementation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Autofac;
using comentapp.persistence;
using Comentapp.AuthenticationManager.Endpoint.Mapper;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Host.ConfigureContainer<ContainerBuilder>((context,containerBuilder) =>
{
    containerBuilder.RegisterModule(new DatabaseModule(context.Configuration));
});

builder.Services.AddDataProtection()
    .SetApplicationName("ComentAppAuth")
    .PersistKeysToDbContext<ComentappDbContext>();

builder.Services.Configure<EmailOptions>(
    builder.Configuration.GetSection(EmailOptions.Section));


builder.Services.AddScoped<AppCookieEvents>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IEmailConfirmationService, EmailConfirmationService>();
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailTemplateRenderer, EmailTemplateRenderer>();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<AuthenticationMapperProfile>();
});

builder.Services.AddAuthentication(options => {
    options.DefaultScheme = "AppCookie";
    //options.DefaultChallengeScheme = "oidc";
}).AddCookie("AppCookie", options =>
{
    options.Cookie.Name = "__Host-app_session";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.Path = "/";

    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

    options.EventsType = typeof(AppCookieEvents);

    options.Events ??= new CookieAuthenticationEvents();

    options.Events.OnRedirectToLogin = ctx =>
    {
        if (ctx.Request.Path.StartsWithSegments("/api"))
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }

        ctx.Response.Redirect(ctx.RedirectUri);
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseHttpsRedirection();
// app.UseAuthorization();

app.MapControllers();
app.Run();