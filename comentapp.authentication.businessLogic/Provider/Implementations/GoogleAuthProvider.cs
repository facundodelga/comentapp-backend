using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    /// <summary>
    /// Placeholder <see cref="IAuthProvider"/> for Google OAuth sign-in.
    /// Registered in DI so <see cref="IAuthProviderFactory"/> can resolve <c>"google"</c>,
    /// but not yet implemented — no controller endpoint calls it.
    /// Tracked in <c>spec/features/feature-google-auth.md</c>.
    /// </summary>
    public class GoogleAuthProvider : IAuthProvider
    {
        /// <inheritdoc />
        public string ProviderName => "google";

        /// <inheritdoc />
        /// <exception cref="NotImplementedException">Always thrown; Google OAuth authentication is not yet implemented.</exception>
        public Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request)
        {
            throw new NotImplementedException("Google authentication is not implemented yet. See spec/features/feature-google-auth.md.");
        }
    }
}
