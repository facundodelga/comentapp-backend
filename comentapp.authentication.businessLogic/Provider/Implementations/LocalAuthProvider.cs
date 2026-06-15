using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    public class LocalAuthProvider : IAuthProvider
    {
        public string ProviderName => "local";

        public Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request)
        {
            throw new NotImplementedException();
        }
    }
}
