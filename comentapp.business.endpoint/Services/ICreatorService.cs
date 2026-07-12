using comentapp.persistence.Models;

namespace comentapp.business.endpoint.Services
{
    public enum CreatorErrorCode
    {
        None = 0,
        AlreadyCreator,
        NameTaken,
    }

    public class CreatorResult
    {
        public required bool Success { get; init; }
        public Creator? Creator { get; init; }
        public bool IsConnected { get; init; }
        public CreatorErrorCode Error { get; init; }
    }

    public interface ICreatorService
    {
        /// <summary>Paso 2: registra al usuario como creador. Un creador por usuario, creatorName único.</summary>
        Task<CreatorResult> RegisterAsync(int userId, string creatorName);

        /// <summary>Devuelve el creador del usuario (con estado de conexión MP), o null si no es creador.</summary>
        Task<CreatorResult?> GetByUserIdAsync(int userId);
    }
}
