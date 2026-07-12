namespace comentapp.infrastructure.Options
{
    /// <summary>
    /// Credenciales de la aplicación Mercado Pago (marketplace).
    /// Valores reales van en user-secrets / variables de entorno, nunca en appsettings.json.
    /// El <see cref="AccessToken"/> es el token de la propia app (para OAuth / pruebas);
    /// los pagos de cada creador usan el access token OAuth del creador, no éste.
    /// </summary>
    public class MercadoPagoOptions
    {
        public const string Section = "MercadoPago";

        public string AccessToken { get; init; } = string.Empty;
        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;

        /// <summary>URL de retorno del connect OAuth del creador (redirect_uri registrado en MP).</summary>
        public string OAuthRedirectUri { get; init; } = string.Empty;

        /// <summary>
        /// Comisión de la app en porcentaje sobre el monto de la donación (ej: 3 = 3%).
        /// TODO: migrar a la entidad <c>Setting</c> para poder cambiarla sin redeploy.
        /// </summary>
        public decimal MarketplaceFeePercent { get; init; } = 3m;

        /// <summary>URL base pública de esta API (business), para armar el notification_url del webhook.</summary>
        public string ApiBaseUrl { get; init; } = string.Empty;
    }
}
