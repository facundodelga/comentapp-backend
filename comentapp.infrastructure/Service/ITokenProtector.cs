namespace comentapp.infrastructure.Service
{
    /// <summary>
    /// Cifra/descifra secretos en reposo (tokens OAuth de creadores) con ASP.NET DataProtection.
    /// Las claves se persisten en la DB vía PersistKeysToDbContext.
    /// </summary>
    public interface ITokenProtector
    {
        string Protect(string plaintext);
        string Unprotect(string ciphertext);
    }
}
