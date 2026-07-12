using MercadoPago.Client;
using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;

namespace comentapp.infrastructure.Service.Implementation
{
    /// <summary>
    /// Crea preferences de Checkout Pro con split de marketplace.
    /// El pago se crea con el access token OAuth del creador (collector) vía
    /// <see cref="RequestOptions.AccessToken"/>, y <c>MarketplaceFee</c> retiene la
    /// comisión de la app. NO usa el access token global de la app.
    /// </summary>
    public class MercadoPagoPreferenceService : IMercadoPagoPreferenceService
    {
        public async Task<CreatePreferenceResult> CreateAsync(CreatePreferenceInput input)
        {
            ArgumentNullException.ThrowIfNull(input);

            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = input.Title,
                        Quantity = 1,
                        CurrencyId = "ARS",
                        UnitPrice = input.Amount,
                    },
                },
                MarketplaceFee = input.MarketplaceFee,
                ExternalReference = input.ExternalReference,
                NotificationUrl = input.NotificationUrl,
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = input.SuccessUrl,
                    Failure = input.FailureUrl,
                    Pending = input.PendingUrl,
                },
                AutoReturn = "approved",
            };

            // Token OAuth del creador: la plata va a su cuenta, la app retiene MarketplaceFee.
            var requestOptions = new RequestOptions { AccessToken = input.CreatorAccessToken };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request, requestOptions);

            return new CreatePreferenceResult
            {
                PreferenceId = preference.Id,
                InitPoint = preference.InitPoint,
                SandboxInitPoint = preference.SandboxInitPoint,
            };
        }
    }
}
