using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    public class GoogleAuthProvider : IAuthProvider
    {
        public string ProviderName => "google";

        public Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
