using comentapp.authentication.businessLogic.Core;
using comentapp.persistence.Models;

namespace comentapp.authentication.businessLogic.Services
{
    /// <summary>
    /// Issues and manages JWT access tokens and rotating refresh tokens.
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Generates a signed JWT access token for the given user.
        /// </summary>
        /// <param name="user">The authenticated user the token is issued for.</param>
        /// <returns>The encoded JWT access token string.</returns>
        string GenerateAccessToken(User user);

        /// <summary>
        /// Creates a new, unsaved refresh token entity for the given user.
        /// </summary>
        /// <param name="userId">The id of the user the refresh token belongs to.</param>
        /// <returns>A new <see cref="RefreshToken"/> instance (not yet persisted).</returns>
        RefreshToken GenerateRefreshToken(int userId);

        /// <summary>
        /// Rotates a refresh token: validates it, revokes the old one, and issues a new
        /// access/refresh token pair.
        /// </summary>
        /// <param name="refreshToken">The refresh token value presented by the client.</param>
        /// <returns>
        /// A successful <see cref="Result{T}"/> with the new <see cref="AuthTokens"/> pair,
        /// or a failure result (error code 401) if the token is invalid or expired.
        /// </returns>
        Task<Result<AuthTokens>> RefreshAsync(string refreshToken);

        /// <summary>
        /// Revokes a refresh token so it can no longer be used to obtain new tokens.
        /// Does nothing if the token does not exist.
        /// </summary>
        /// <param name="refreshToken">The refresh token value to revoke.</param>
        Task RevokeAsync(string refreshToken);

        /// <summary>
        /// Persists a newly generated refresh token.
        /// </summary>
        /// <param name="refreshToken">The refresh token entity to save.</param>
        Task SaveRefreshTokenAsync(RefreshToken refreshToken);
    }
}
