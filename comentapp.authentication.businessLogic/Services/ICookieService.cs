using comentapp.authentication.businessLogic.Core;
using Microsoft.AspNetCore.Http;

namespace comentapp.authentication.businessLogic.Services
{
    /// <summary>
    /// Interfaz para manejar cookies de autenticación.
    /// Soporta tanto autenticación propia como OAuth.
    /// </summary>
    public interface ICookieService
    {
        /// <summary>
        /// Establecer cookies de autenticación después de login exitoso.
        /// Crea un principal con los datos del usuario y tokens.
        /// </summary>
        /// <param name="response">HttpResponse para establecer las cookies</param>
        /// <param name="tokens">Tokens JWT y datos del usuario</param>
        Task SetAuthCookies(HttpResponse response, AuthTokens tokens);

        /// <summary>
        /// Limpiar todas las cookies de autenticación (logout).
        /// </summary>
        /// <param name="response">HttpResponse para limpiar las cookies</param>
        Task ClearAuthCookies(HttpResponse response);

        /// <summary>
        /// Obtener el token de refresco desde las cookies.
        /// Se usa para renovar el access token cuando expira.
        /// </summary>
        /// <param name="request">HttpRequest para leer las cookies</param>
        /// <returns>Token de refresco o null si no existe</returns>
        string? GetRefreshToken(HttpRequest request);
    }
}
