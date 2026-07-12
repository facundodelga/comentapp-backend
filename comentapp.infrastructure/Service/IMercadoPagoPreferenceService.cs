namespace comentapp.infrastructure.Service
{
    /// <summary>
    /// Datos para crear una preference de Checkout Pro por una donación con comentario.
    /// </summary>
    public class CreatePreferenceInput
    {
        /// <summary>Access token OAuth del creador que cobra (collector). El dinero va a su cuenta.</summary>
        public required string CreatorAccessToken { get; init; }

        /// <summary>Monto total de la donación pagado por el usuario.</summary>
        public required decimal Amount { get; init; }

        /// <summary>Comisión de la app en pesos (monto absoluto, no porcentaje). Ej: Amount * 0.03.</summary>
        public required decimal MarketplaceFee { get; init; }

        /// <summary>Título mostrado en el checkout (ej: "Donación a {creador}").</summary>
        public required string Title { get; init; }

        /// <summary>Referencia externa: id del Payment/Comment propio para conciliar en el webhook.</summary>
        public required string ExternalReference { get; init; }

        /// <summary>URL del webhook que recibe la notificación de pago (verificación server-side).</summary>
        public required string NotificationUrl { get; init; }

        public required string SuccessUrl { get; init; }
        public required string FailureUrl { get; init; }
        public required string PendingUrl { get; init; }
    }

    public class CreatePreferenceResult
    {
        public required string PreferenceId { get; init; }
        /// <summary>URL de producción a la que redirigir al usuario.</summary>
        public required string InitPoint { get; init; }
        /// <summary>URL de sandbox para pruebas.</summary>
        public required string SandboxInitPoint { get; init; }
    }

    public interface IMercadoPagoPreferenceService
    {
        Task<CreatePreferenceResult> CreateAsync(CreatePreferenceInput input);
    }
}
