namespace comentapp.authentication.businessLogic.Provider
{
    /// <summary>
    /// Resolves the <see cref="IAuthProvider"/> registered for a given provider name.
    /// </summary>
    public interface IAuthProviderFactory
    {
        /// <summary>
        /// Gets the <see cref="IAuthProvider"/> matching the given name (case-insensitive).
        /// </summary>
        /// <param name="providerName">The provider name (e.g. <c>"local"</c>, <c>"google"</c>).</param>
        /// <returns>The matching <see cref="IAuthProvider"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when no provider matches <paramref name="providerName"/>.</exception>
        IAuthProvider GetProvider(string providerName);
    }
}
