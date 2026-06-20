// Agregar a Program.cs cuando implementar Google OAuth

/* 
// En appsettings.json:
{
  "Google": {
    "ClientId": "your-client-id.apps.googleusercontent.com",
    "ClientSecret": "your-client-secret"
  },
  "Cors": {
    "AllowedOrigins": "https://localhost:3000"
  }
}

// En Program.cs, descomenta lo siguiente:

builder.Services.AddAuthentication(...)
    .AddCookie("AppCookie", ...)
    .AddCookie("ExternalCookie", ...)
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Google:ClientSecret"]!;

        // Cookie temporal para la autenticación externa
        options.SignInScheme = "ExternalCookie";

        // Scopes de Google
        options.Scope.Clear();
        options.Scope.Add("profile");
        options.Scope.Add("email");

        // Eventos para manejar el flujo
        options.Events = new Google.Api.Gax.Grpc.ApiCallSettings()
        {
            OnTicketReceived = async context =>
            {
                // Aquí procesar el usuario de Google
                // 1. Extraer información del usuario
                var identity = context.Principal?.Identity as ClaimsIdentity;
                var email = identity?.FindFirst(ClaimTypes.Email)?.Value;
                var name = identity?.FindFirst(ClaimTypes.Name)?.Value;

                // 2. Buscar o crear el usuario en DB
                // 3. Crear AuthTokens
                // 4. SignInAsync con el esquema "AppCookie"

                await Task.CompletedTask;
            }
        };
    });

// Endpoint para iniciar flujo OAuth:
[HttpGet("login/google")]
public IActionResult LoginGoogle(string? returnUrl = null)
{
    var redirectUrl = Url.Action("GoogleCallback", "Authentication", new { returnUrl });
    var properties = new AuthenticationProperties 
    { 
        RedirectUri = redirectUrl 
    };
    return Challenge(properties, "Google");
}

// Callback después de que Google autentica:
[HttpGet("login/google/callback")]
public async Task<IActionResult> GoogleCallback(string? returnUrl = null)
{
    var result = await HttpContext.AuthenticateAsync("ExternalCookie");
    if (!result.Succeeded)
        return Unauthorized();

    var email = result.Principal?.FindFirst(ClaimTypes.Email)?.Value;

    // Buscar o crear usuario
    var user = await _userService.FindOrCreateFromGoogleAsync(email!);

    // Generar tokens
    var tokens = _tokenService.GenerateTokens(user);

    // Establecer cookies
    _cookieService.SetAuthCookies(Response, tokens);

    // Limpiar cookie externa
    await HttpContext.SignOutAsync("ExternalCookie");

    return Redirect(returnUrl ?? "/");
}
*/
