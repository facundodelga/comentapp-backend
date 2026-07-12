namespace comentapp.business.endpoint.DTOs
{
    public class DonationCommentResponse
    {
        public required int DonationId { get; init; }
        public required string PreferenceId { get; init; }
        public required string CheckoutUrl { get; init; }
    }
}
