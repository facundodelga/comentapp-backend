using System.Security.Cryptography;
using comentapp.infrastructure.Service;
using comentapp.persistence.Models;
using comentapp.persistence.Repository;
using Microsoft.Extensions.Logging;

namespace comentapp.business.endpoint.Services
{
    /// <summary>
    /// Implementa el flujo OAuth de Mercado Pago Connect.
    /// El state anti-CSRF transporta la identidad del creador porque el callback vuelve
    /// desde MP como navegación cross-site y la cookie de sesión (SameSite=Strict) no viaja.
    /// </summary>
    public class MercadoPagoConnectService(
        ILogger<MercadoPagoConnectService> logger,
        ICreatorRepository creatorRepository,
        ICreatorMercadoPagoAccountRepository accountRepository,
        IMercadoPagoOAuthStateRepository stateRepository,
        IMercadoPagoOAuthService oauthService,
        ITokenProtector tokenProtector) : IMercadoPagoConnectService
    {
        private static readonly TimeSpan StateLifetime = TimeSpan.FromMinutes(10);

        public async Task<ConnectStartResult> StartConnectAsync(int userId)
        {
            var creator = await creatorRepository.GetByUserIdAsync(userId);
            if (creator is null)
            {
                return new ConnectStartResult { Success = false, Error = "El usuario no es un creador." };
            }

            var state = GenerateState();

            await stateRepository.AddAsync(new MercadoPagoOAuthState
            {
                State = state,
                CreatorId = creator.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(StateLifetime),
                Used = false,
            });
            await stateRepository.SaveChangesAsync();

            var url = oauthService.BuildAuthorizationUrl(state);
            return new ConnectStartResult { Success = true, AuthorizationUrl = url };
        }

        public async Task<ConnectCallbackResult> HandleCallbackAsync(string code, string state)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
            {
                return new ConnectCallbackResult { Success = false, Error = "Faltan parámetros code/state." };
            }

            var stateEntry = await stateRepository.GetByStateAsync(state);
            if (stateEntry is null || stateEntry.Used || stateEntry.ExpiresAt < DateTime.UtcNow)
            {
                logger.LogWarning("Callback MP con state inválido/expirado/usado.");
                return new ConnectCallbackResult { Success = false, Error = "State inválido o expirado." };
            }

            // Consumir el state antes de cualquier efecto para evitar replay.
            stateEntry.Used = true;
            stateRepository.Update(stateEntry);
            await stateRepository.SaveChangesAsync();

            MercadoPagoTokenResult tokens;
            try
            {
                tokens = await oauthService.ExchangeCodeAsync(code);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falló el intercambio de code OAuth con Mercado Pago.");
                return new ConnectCallbackResult { Success = false, Error = "No se pudo completar la conexión con Mercado Pago." };
            }

            var account = await accountRepository.GetByCreatorIdAsync(stateEntry.CreatorId);
            var now = DateTime.UtcNow;
            var expiresAt = now.AddSeconds(tokens.ExpiresInSeconds);

            if (account is null)
            {
                account = new CreatorMercadoPagoAccount
                {
                    CreatorId = stateEntry.CreatorId,
                    MpUserId = tokens.MpUserId,
                    AccessTokenEncrypted = tokenProtector.Protect(tokens.AccessToken),
                    RefreshTokenEncrypted = tokenProtector.Protect(tokens.RefreshToken),
                    TokenExpiresAt = expiresAt,
                    PublicKey = tokens.PublicKey,
                    IsConnected = true,
                    ConnectedAt = now,
                    UpdatedAt = now,
                };
                await accountRepository.AddAsync(account);
            }
            else
            {
                account.MpUserId = tokens.MpUserId;
                account.AccessTokenEncrypted = tokenProtector.Protect(tokens.AccessToken);
                account.RefreshTokenEncrypted = tokenProtector.Protect(tokens.RefreshToken);
                account.TokenExpiresAt = expiresAt;
                account.PublicKey = tokens.PublicKey;
                account.IsConnected = true;
                account.UpdatedAt = now;
                accountRepository.Update(account);
            }

            await accountRepository.SaveChangesAsync();

            logger.LogInformation("Creador {CreatorId} conectó su cuenta MP {MpUserId}.", stateEntry.CreatorId, tokens.MpUserId);
            return new ConnectCallbackResult { Success = true };
        }

        public async Task<ConnectStatusResult> GetStatusAsync(int userId)
        {
            var creator = await creatorRepository.GetByUserIdAsync(userId);
            if (creator is null)
            {
                return new ConnectStatusResult { IsCreator = false, Connected = false };
            }

            var account = await accountRepository.GetByCreatorIdAsync(creator.Id);
            return new ConnectStatusResult
            {
                IsCreator = true,
                Connected = account?.IsConnected ?? false,
                AccountId = account?.IsConnected == true ? account.MpUserId : null,
            };
        }

        public async Task<bool> DisconnectAsync(int userId)
        {
            var creator = await creatorRepository.GetByUserIdAsync(userId);
            if (creator is null)
            {
                return false;
            }

            var account = await accountRepository.GetByCreatorIdAsync(creator.Id);
            if (account is null || !account.IsConnected)
            {
                return false;
            }

            // No borramos el registro: marcamos desconectado e invalidamos los tokens.
            account.IsConnected = false;
            account.AccessTokenEncrypted = string.Empty;
            account.RefreshTokenEncrypted = string.Empty;
            account.UpdatedAt = DateTime.UtcNow;
            accountRepository.Update(account);
            await accountRepository.SaveChangesAsync();

            return true;
        }

        private static string GenerateState()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToHexString(bytes);
        }
    }
}
