namespace comentapp.business.endpoint.Services
{
    public class ConnectStartResult
    {
        public required bool Success { get; init; }
        public string? AuthorizationUrl { get; init; }
        public string? Error { get; init; }
    }

    public class ConnectCallbackResult
    {
        public required bool Success { get; init; }
        public string? Error { get; init; }
    }

    public class ConnectStatusResult
    {
        /// <summary>Null si el usuario no es creador (no completó el paso 2).</summary>
        public bool? IsCreator { get; init; }
        public bool Connected { get; init; }
        /// <summary>Id público de la cuenta MP del creador (no es un token).</summary>
        public string? AccountId { get; init; }
    }

    /// <summary>
    /// Orquesta el flujo OAuth de Mercado Pago Connect para vincular la cuenta MP de un creador.
    /// </summary>
    public interface IMercadoPagoConnectService
    {
        /// <summary>Inicia la conexión: genera y persiste el state anti-CSRF y arma la URL de autorización.</summary>
        Task<ConnectStartResult> StartConnectAsync(int userId);

        /// <summary>Procesa el callback de MP: valida el state, canjea el code y guarda los tokens cifrados.</summary>
        Task<ConnectCallbackResult> HandleCallbackAsync(string code, string state);

        /// <summary>Estado de conexión MP del creador del usuario.</summary>
        Task<ConnectStatusResult> GetStatusAsync(int userId);

        /// <summary>Desconecta la cuenta MP del creador del usuario. Devuelve false si no había conexión.</summary>
        Task<bool> DisconnectAsync(int userId);
    }
}
