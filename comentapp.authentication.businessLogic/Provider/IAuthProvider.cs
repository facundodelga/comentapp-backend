using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;

namespace comentapp.authentication.businessLogic.Provider
{
    /// <summary>
    /// Strategy abstraction for an authentication mechanism (e.g. local email/password,
    /// Google OAuth). Implementations are resolved by name via <see cref="IAuthProviderFactory"/>.
    /// </summary>
    public interface IAuthProvider
    {
        /// <summary>
        /// Unique, case-insensitive name identifying this provider (e.g. <c>"local"</c>, <c>"google"</c>).
        /// </summary>
        string ProviderName { get; }

        /// <summary>
        /// Authenticates a request using this provider's mechanism and, on success, issues session tokens.
        /// </summary>
        /// <param name="request">The login request data for this provider.</param>
        /// <returns>A successful <see cref="Result{T}"/> with <see cref="AuthTokens"/>, or a failure result.</returns>
        Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request);
    }
}
