namespace comentapp.authentication.businessLogic.Core
{
    /// <summary>
    /// Tokens de autenticación y datos del usuario.
    /// Se retorna después de login exitoso.
    /// </summary>
    public class AuthTokens
    {
        /// <summary>
        /// JWT para autorización en solicitudes subsecuentes.
        /// Típicamente con validez de 15-60 minutos.
        /// </summary>
        public string AccessToken { get; init; } = string.Empty;

        /// <summary>
        /// JWT para renovar el AccessToken cuando expira.
        /// Típicamente con validez de 7 días.
        /// </summary>
        public string RefreshToken { get; init; } = string.Empty;

        /// <summary>
        /// ID único del usuario autenticado.
        /// </summary>
        public int UserId { get; init; }

        /// <summary>
        /// Nombre de usuario (identificador único).
        /// </summary>
        public string? UserName { get; init; }

        /// <summary>
        /// Email del usuario.
        /// </summary>
        public string? Email { get; init; }

        /// <summary>
        /// Proveedor de autenticación: "local", "google", etc.
        /// </summary>
        public string? AuthProvider { get; init; } = "local";
    }
}
