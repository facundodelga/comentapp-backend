

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    public class AuthProviderFactory(IEnumerable<IAuthProvider> providers) : IAuthProviderFactory
    {
        IEnumerable<IAuthProvider> _providers = providers;
        public IAuthProvider GetProvider(string providerName)
        {
            var provider = _providers.FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                throw new NotSupportedException($"Authentication provider '{providerName}' is not supported.");
            }

            return provider;
        }
    }
}

