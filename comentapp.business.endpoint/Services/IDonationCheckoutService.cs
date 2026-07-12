using comentapp.business.endpoint.DTOs;

namespace comentapp.business.endpoint.Services
{
    public enum DonationErrorCode
    {
        None = 0,
        CreatorNotFound,
        CreatorNotConnected,
        CheckoutFailed,
    }

    public class DonationCheckoutResult
    {
        public required bool Success { get; init; }
        public DonationCommentResponse? Response { get; init; }
        public DonationErrorCode Error { get; init; }
    }

    /// <summary>
    /// Crea la intención de donación (Payment + Comment en estado pendiente) y una preference
    /// de Checkout Pro con split marketplace, devolviendo la URL de pago.
    /// </summary>
    public interface IDonationCheckoutService
    {
        Task<DonationCheckoutResult> CreateAsync(int payerUserId, DonationCommentRequest request);
    }
}
