using comentapp.business.endpoint.DTOs;
using comentapp.infrastructure.Options;
using comentapp.infrastructure.Service;
using comentapp.persistence.Models;
using comentapp.persistence.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace comentapp.business.endpoint.Services
{
    public class DonationCheckoutService(
        ILogger<DonationCheckoutService> logger,
        ICreatorRepository creatorRepository,
        ICreatorMercadoPagoAccountRepository accountRepository,
        IPaymentRepository paymentRepository,
        ICommentRepository commentRepository,
        IMercadoPagoOAuthService oauthService,
        IMercadoPagoPreferenceService preferenceService,
        ITokenProtector tokenProtector,
        MercadoPagoOptions options,
        IConfiguration configuration) : IDonationCheckoutService
    {
        public async Task<DonationCheckoutResult> CreateAsync(int payerUserId, DonationCommentRequest request)
        {
            var creator = await creatorRepository.GetByIdAsync(request.CreatorId);
            if (creator is null)
            {
                return Fail(DonationErrorCode.CreatorNotFound);
            }

            var account = await accountRepository.GetByCreatorIdAsync(creator.Id);
            if (account is null || !account.IsConnected)
            {
                // Regla: un creador sin conexión MP no puede recibir donaciones.
                return Fail(DonationErrorCode.CreatorNotConnected);
            }

            var now = DateTime.UtcNow;
            var fee = decimal.Round(request.Amount * options.MarketplaceFeePercent / 100m, 2);

            // 1) Payment pendiente. Es principal del 1:1 con Comment (FK vive en Comment.PaymentId).
            var payment = new Payment
            {
                PaymentStatusId = (int)PaymentStatusIds.Pending,
                Amount = request.Amount,
                MarketplaceFee = fee,
                NetReceivedAmount = request.Amount - fee,
                CreatorId = creator.Id,
                UserId = payerUserId,
                CreatedAt = now,
                UpdatedAt = now,
                MercadoPagoId = string.Empty,
                PreferenceId = string.Empty,
            };
            await paymentRepository.AddAsync(payment);
            await paymentRepository.SaveChangesAsync();

            // 2) Comment sin confirmar, ligado al Payment.
            var comment = new Comment
            {
                CommentText = request.Comment,
                CreatedAt = now,
                UserId = payerUserId,
                CreatorId = creator.Id,
                PaymentId = payment.Id,
            };
            await commentRepository.AddAsync(comment);
            await commentRepository.SaveChangesAsync();

            payment.CommentId = comment.Id;
            paymentRepository.Update(payment);
            await paymentRepository.SaveChangesAsync();

            // 3) Token válido del creador (refresca si venció) y preference con split.
            string creatorAccessToken;
            try
            {
                creatorAccessToken = await GetValidAccessTokenAsync(account);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "No se pudo obtener un access token válido del creador {CreatorId}.", creator.Id);
                return Fail(DonationErrorCode.CheckoutFailed);
            }

            var frontendBase = configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
            var externalReference = payment.Id.ToString();

            CreatePreferenceResult preference;
            try
            {
                preference = await preferenceService.CreateAsync(new CreatePreferenceInput
                {
                    CreatorAccessToken = creatorAccessToken,
                    Amount = request.Amount,
                    MarketplaceFee = fee,
                    Title = $"Donación a {creator.CreatorName}",
                    ExternalReference = externalReference,
                    NotificationUrl = $"{options.ApiBaseUrl.TrimEnd('/')}/MercadoPago/webhooks",
                    SuccessUrl = $"{frontendBase}/donation/result?status=success&ref={externalReference}",
                    FailureUrl = $"{frontendBase}/donation/result?status=failure&ref={externalReference}",
                    PendingUrl = $"{frontendBase}/donation/result?status=pending&ref={externalReference}",
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Falló la creación de la preference para el Payment {PaymentId}.", payment.Id);
                return Fail(DonationErrorCode.CheckoutFailed);
            }

            payment.PreferenceId = preference.PreferenceId;
            paymentRepository.Update(payment);
            await paymentRepository.SaveChangesAsync();

            return new DonationCheckoutResult
            {
                Success = true,
                Response = new DonationCommentResponse
                {
                    DonationId = payment.Id,
                    PreferenceId = preference.PreferenceId,
                    CheckoutUrl = preference.InitPoint,
                },
            };
        }

        /// <summary>
        /// Descifra el access token del creador; si está por vencer lo renueva con el refresh token
        /// y persiste los nuevos tokens cifrados.
        /// </summary>
        private async Task<string> GetValidAccessTokenAsync(CreatorMercadoPagoAccount account)
        {
            // Margen de 60s para evitar usar un token que vence en el medio de la llamada.
            if (account.TokenExpiresAt > DateTime.UtcNow.AddSeconds(60))
            {
                return tokenProtector.Unprotect(account.AccessTokenEncrypted);
            }

            var refreshToken = tokenProtector.Unprotect(account.RefreshTokenEncrypted);
            var refreshed = await oauthService.RefreshAsync(refreshToken);

            account.AccessTokenEncrypted = tokenProtector.Protect(refreshed.AccessToken);
            account.RefreshTokenEncrypted = tokenProtector.Protect(refreshed.RefreshToken);
            account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(refreshed.ExpiresInSeconds);
            account.UpdatedAt = DateTime.UtcNow;
            accountRepository.Update(account);
            await accountRepository.SaveChangesAsync();

            return refreshed.AccessToken;
        }

        private static DonationCheckoutResult Fail(DonationErrorCode error) =>
            new() { Success = false, Error = error };
    }
}
