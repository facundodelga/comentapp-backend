namespace comentapp.business.endpoint.DTOs
{
    public class CreatorResponse
    {
        public required int Id { get; init; }
        public required string CreatorName { get; init; }
        public required int UserId { get; init; }
        public string? Description { get; init; }

        /// <summary>True si completó el connect OAuth (paso 3) y puede recibir donaciones.</summary>
        public required bool MercadoPagoConnected { get; init; }
    }
}
