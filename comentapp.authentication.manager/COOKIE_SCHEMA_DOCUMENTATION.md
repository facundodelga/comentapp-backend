# Esquema de Cookies - Documentación

## Descripción General

La aplicación implementa un esquema de cookies de múltiples capas que soporta:
1. **Autenticación propia** (email + contraseña)
2. **Autenticación externa** (Google OAuth - en desarrollo)
3. **Cookies de sessión** con deslizamiento automático
4. **Tokens JWT** almacenados de forma segura en claims

---

## Arquitectura de Cookies

### 1. Cookie Principal: `AppCookie`

**Propósito:** Mantener la sesión autenticada del usuario

**Características:**
- Nombre: `__Host-app_session`
- HttpOnly: `true` (JS no puede leerla)
- Secure: `true` (solo HTTPS)
- SameSite: `Strict` (máxima protección CSRF)
- Expiración deslizante: 8 horas
- Path: `/` (accesible en toda la aplicación)

**Contenido (Claims):**
```
- NameIdentifier: ID del usuario
- Name: Nombre del usuario
- Email: Email del usuario
- access_token: JWT para autorización
- refresh_token: JWT para renovación
- auth_provider: "local" o "google"
- authenticated_at: Unix timestamp
- absolute_exp_utc: Expiración máxima de sesión (24h)
```

### 2. Cookie Externa: `ExternalCookie`

**Propósito:** Cookie temporal durante flujo OAuth (Google)

**Características:**
- Nombre: `__Host-external_session`
- SameSite: `Lax` (permite redirecciones desde Google)
- Expiración: 30 minutos
- Se limpia automáticamente después del login

---

## Flujo de Autenticación

### Autenticación Propia (Local)

```
1. POST /authentication/login
   └─> Email + Contraseña

2. Provider local valida credenciales

3. Si válidas:
   └─> Generar AuthTokens (AccessToken + RefreshToken)
   └─> Llamar CookieService.SetAuthCookies()
   └─> Crear principal con claims
   └─> SignInAsync("AppCookie", principal)

4. Respuesta al cliente
   ├─> Set-Cookie: __Host-app_session=<valor>; HttpOnly; Secure; SameSite=Strict
   └─> JSON con información del usuario
```

### Autenticación Google OAuth (Futuro)

```
1. GET /authentication/login/google
   └─> Challenge("Google")
   └─> Redirige a Google

2. Google autentica al usuario

3. Callback: GET /authentication/login/google/callback
   ├─> AuthenticateAsync("ExternalCookie")
   ├─> Extraer email y datos de Google
   ├─> Buscar o crear usuario en BD
   ├─> Generar AuthTokens
   ├─> SignInAsync("AppCookie", principal)
   ├─> SignOutAsync("ExternalCookie") [limpiar]
   └─> Redirect al inicio

4. Cookie __Host-app_session está configurada
```

---

## Flujo de Validación de Sesión

### En cada solicitud autenticada:

```
1. Middleware lee cookie "AppCookie"

2. AppCookieEvents.ValidatePrincipal()
   ├─> Verificar existencia de claim "absolute_exp_utc"
   ├─> Parsear Unix timestamp
   ├─> Comparar con DateTimeOffset.UtcNow
   ├─> Si expiró: RejectPrincipal() + SignOutAsync()
   └─> Si válida: Continuar

3. User.Identity.IsAuthenticated = true
   └─> Autorización basada en roles/políticas
```

---

## Endpoints Clave

### Login (Autenticación Propia)
```http
POST /authentication/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123"
}

Response: 200 OK
Set-Cookie: __Host-app_session=...; HttpOnly; Secure; SameSite=Strict
{
  "isSuccess": true,
  "value": {
	"userId": 1,
	"userName": "john_doe",
	"email": "user@example.com"
  }
}
```

### Login Google (Futuro)
```http
GET /authentication/login/google

Response: 302 Redirect
Location: https://accounts.google.com/o/oauth2/v2/auth?...
Set-Cookie: __Host-external_session=...; HttpOnly; Secure; SameSite=Lax
```

### Logout
```http
POST /authentication/logout

Response: 200 OK
Set-Cookie: __Host-app_session=; expires=Thu, 01 Jan 1970 00:00:00 GMT
{
  "isSuccess": true,
  "message": "Logged out successfully"
}
```

### Verificar Sesión
```http
GET /session/verify

Response: 200 OK (si autenticado)
{
  "isAuthenticated": true,
  "user": {
	"id": 1,
	"email": "user@example.com",
	"name": "John Doe"
  }
}

Response: 401 Unauthorized (si no autenticado)
```

---

## Configuración en appsettings.json

```json
{
  "Jwt": {
	"AccessTokenExpirationMinutes": 15,
	"RefreshTokenExpirationDays": 7
  },
  "Google": {
	"ClientId": "your-client-id.apps.googleusercontent.com",
	"ClientSecret": "your-client-secret"
  },
  "Cors": {
	"AllowedOrigins": "https://localhost:3000"
  }
}
```

---

## Seguridad

### Protecciones Implementadas

1. **HttpOnly Flag:** Los tokens no son accesibles desde JavaScript
2. **Secure Flag:** Las cookies solo se transmiten por HTTPS
3. **SameSite=Strict:** Protección contra ataques CSRF
4. **Expiración Deslizante:** Se renueva automáticamente en cada solicitud
5. **Expiración Absoluta:** Máximo 24 horas aunque haya actividad
6. **Data Protection:** Cifrado automático por ASP.NET Core
7. **Token Validation:** Verificación de expiración en cada solicitud

---

## Problemas Comunes y Soluciones

### Problema: Las cookies no se envían desde el frontend

**Causa:** Falta `credentials: 'include'` en fetch/axios

**Solución:**
```javascript
// Fetch
fetch('/api/endpoint', {
  credentials: 'include'  // Incluir cookies
})

// Axios
axios.defaults.withCredentials = true;
```

### Problema: CORS error al intentar autenticar

**Causa:** Falta configuración de CORS con `AllowCredentials`

**Solución:** Program.cs ya lo configura, verificar orígenes permitidos

### Problema: Cookie no se establece después de login

**Causa:** Protocol mismatch (HTTP vs HTTPS)

**Solución:** 
- En desarrollo: Desactivar Secure flag temporalmente
- En producción: Usar HTTPS

### Problema: Sesión expira demasiado rápido

**Solución:** Aumentar `ExpireTimeSpan` en Program.cs o `absolute_exp_utc` claim

---

## Testing de Cookies

### Con Postman

1. En la primera solicitud de login, Postman guarda la cookie
2. Las siguientes solicitudes la incluyen automáticamente
3. Verificar en "Cookies" tab

### Con cURL

```bash
# Login y guardar cookies
curl -X POST https://localhost:7001/authentication/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"pass123"}' \
  -c cookies.txt

# Usar cookies en siguiente solicitud
curl -X GET https://localhost:7001/session/verify \
  -b cookies.txt
```

### Con JavaScript/Fetch

```javascript
// Login
const response = await fetch('/authentication/login', {
  method: 'POST',
  credentials: 'include',  // IMPORTANTE
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ email: 'user@example.com', password: 'pass123' })
});

// Las cookies se guardan automáticamente

// Verificar sesión
const sessionResponse = await fetch('/session/verify', {
  credentials: 'include'  // IMPORTANTE
});
```

---

## Referencias

- [ASP.NET Core Authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication)
- [Cookie Authentication Scheme](https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authentication.cookies)
- [HTTP Cookie Security](https://owasp.org/www-community/attacks/csrf)
- [SameSite Cookie Explained](https://web.dev/samesite-cookies-explained/)
