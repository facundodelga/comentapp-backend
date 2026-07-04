# Backend - contexto tecnico

## Ubicacion

Solucion en `comentapp-backend`.

Archivo solucion: `comentapp-backend.slnx`.

## Stack

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server LocalDB por defecto
- Autofac como contenedor DI
- AutoMapper
- ASP.NET Core Cookie Authentication
- JWT interno para access token guardado como claim en cookie
- Refresh tokens persistidos en DB
- DataProtection keys en DB
- MailKit para email SMTP
- Swagger en development
- Dockerfiles para endpoints

## Proyectos

- `comentapp.authentication.manager`: API de autenticacion.
- `comentapp.authentication.businessLogic`: casos de uso de auth, providers, tokens, cookies.
- `comentapp.business.endpoint`: API de negocio inicial para comentarios.
- `comentapp.persistence`: EF Core, modelos, repositorios, migraciones.
- `comentapp.infrastructure`: email, JWT options, templates.

## Configuracion principal

`comentapp.authentication.manager/appsettings.json` incluye:

- `ConnectionStrings:DefaultConnection`: `Server=(localdb)\MSSQLLocalDB;Database=COMENTAPP;Trusted_Connection=True;`
- `Frontend:BaseUrl`: `http://localhost:5173`
- `Email`: SMTP local en `localhost:1025`
- `Jwt`: secret, issuer, audience, expiracion access/refresh

`comentapp.business.endpoint/appsettings.json` solo tiene logging/hosts. Si necesita DB, debe agregar connection string o compartir configuracion.

## Persistencia

`ComentappDbContext`:

- `Users`
- `DataProtectionKeys`
- `RefreshTokens`
- `Settings`

Tambien configura entidades:

- `Creator`
- `Comment`

Nota: aunque `Creator` y `Comment` estan configuradas en `OnModelCreating`, no hay `DbSet<Creator>` ni `DbSet<Comment>` publicos. EF puede mapearlas por configuracion, pero para queries/repositorios conviene agregarlos.

### Modelos

`User`:

- Id
- Name
- Surname
- UserName
- Email
- PasswordHash
- CreatedDate
- IsEmailConfirmed
- LastModifiedDate
- Comments

`Creator`:

- Id
- CreatorName unico
- UserId unico
- MercadoPagoAccount requerido
- links sociales opcionales
- Description
- User
- Comments

`Comment`:

- Id
- CommentText max 300
- CreatedAt default `GETUTCDATE()`
- UserId
- CreatorId
- User
- Creator

`RefreshToken` y `Setting` existen para sesion/configuracion.

## Autenticacion API

Controller: `comentapp.authentication.manager/Controllers/AuthenticationController.cs`

Base route: `/Authentication` por `[Route("[controller]")]`.
Con proxy frontend se consume como `/api/authentication/...`.

Endpoints:

- `GET /Authentication`: requiere auth. Health simple.
- `POST /Authentication/register`: registra usuario, valida email/username duplicado, envia email de confirmacion.
- `POST /Authentication/confirm-email`: confirma email con token.
- `POST /Authentication/login`: auth local, setea cookies.
- `POST /Authentication/refresh`: rota refresh token, renueva cookie.
- `POST /Authentication/logout`: revoca refresh token y limpia cookie.
- `GET /Authentication/me`: requiere auth, devuelve `name` y `email`.

## Cookies y sesion

`Program.cs` configura:

- esquema default `AppCookie`
- cookie `__Host-app_session`
- `SameSite=Strict`
- `SecurePolicy=Always`
- `HttpOnly=true`
- expiracion deslizante 8 horas
- cookie externa `__Host-external_session` para OAuth futuro
- DataProtection keys persistidas en DB
- CORS con credenciales para origen frontend

`CookieService` crea claims:

- `NameIdentifier`
- `Name`
- `Email`
- `access_token`
- `refresh_token`
- `auth_provider`
- `authenticated_at`
- `absolute_exp_utc`

Deuda tecnica:

- `SetAuthCookies` y `ClearAuthCookies` son `async void`; deberian devolver `Task`.
- `TokenService.RevokeAsync` usa `ContinueWith(async ...)`; puede ocultar errores. Mejor `await` normal.
- `GET /Authentication/me` no devuelve `id` ni rol creador, pero frontend/specs lo necesitan.

## Email

`UserService.RegisterUser`:

1. Hashea password.
2. Crea usuario.
3. Genera token de confirmacion.
4. Arma URL `${Frontend:BaseUrl}/confirm-email?email=...&token=...`.
5. Renderiza `Templates/Emails/confirm-email.html`.
6. Envia email SMTP.
7. Guarda cambios.

## Business endpoint

Controller: `comentapp.business.endpoint/Controllers/CommentsController.cs`

Base route: `/Comments`.

Endpoints actuales:

- `GET /Comments`: devuelve string de health.
- `POST /Comments`: recibe `CommentRequest`, responde mensaje y eco de comentario.

No persiste comentarios todavia.
No valida usuario autenticado.
No conecta con Mercado Pago.
No emite eventos realtime.

## DI

`AuthenticationBusinessModule` registra:

- `PasswordHasher<User>`
- `AuthProviderFactory`
- providers que implementan `IAuthProvider`
- servicios que terminan en `Service`
- `DatabaseModule`
- `EmailModule`
- `JwtModule`

`DatabaseModule` registra:

- repositorios que terminan en `Repository`
- `DbContextOptions<ComentappDbContext>`
- `ComentappDbContext`

## Migraciones

Existen migraciones:

- `InitialCreate`
- `AddRefreshTokens`
- `AddSettings`
- `CreateCreator`
- `CreateComments`

## Riesgos conocidos

- `comentapp.business.endpoint` llama `UseAuthentication()` sin configurar esquema de auth; puede fallar o no hacer nada util.
- No hay endpoint para `/be-a-creator`, pero frontend lo llama.
- No hay endpoint para generar preferencia Mercado Pago.
- No hay endpoint de callbacks Mercado Pago.
- No hay SignalR configurado.
- No hay tests backend en solucion.
