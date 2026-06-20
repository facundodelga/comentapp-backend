using comentapp.authentication.businessLogic.Core;
using comentapp.persistence.Models;

namespace comentapp.authentication.businessLogic.Services
{
    public interface ITokenService
    {        
        string GenerateAccessToken(User user);
        RefreshToken GenerateRefreshToken(int userId);
        Task<Result<AuthTokens>> RefreshAsync(string refreshToken);
        Task RevokeAsync(string refreshToken);
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
    }
}
