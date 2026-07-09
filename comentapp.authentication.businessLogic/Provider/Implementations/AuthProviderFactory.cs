

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    /// <summary>
    /// Default <see cref="IAuthProviderFactory"/> implementation, resolving providers
    /// from the set of <see cref="IAuthProvider"/> instances registered in the DI container.
    /// </summary>
    public class AuthProviderFactory(IEnumerable<IAuthProvider> providers) : IAuthProviderFactory
    {
        private readonly IEnumerable<IAuthProvider> _providers = providers ?? throw new ArgumentNullException(nameof(providers));

        /// <inheritdoc />
        public IAuthProvider GetProvider(string providerName)
        {
            ArgumentException.ThrowIfNullOrEmpty(providerName);

            var provider = _providers.FirstOrDefault(p => p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase));

            if (provider == null)
            {
                throw new NotSupportedException($"Authentication provider '{providerName}' is not supported.");
            }

            return provider;
        }
    }
}

