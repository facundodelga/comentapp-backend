using System;
using System.ComponentModel.DataAnnotations;

namespace comentapp.persistence.Models
{
    public class CreatorMercadoPagoAccount
    {
        public int Id { get; set; }
        public int CreatorId { get; set; }              // 1:1 con Creator

        [StringLength(100)]
        public string MpUserId { get; set; } = string.Empty;   // collector_id de MP (público)

        // Tokens cifrados con DataProtection antes de persistir. Nunca exponer en DTO.
        public string AccessTokenEncrypted { get; set; } = string.Empty;
        public string RefreshTokenEncrypted { get; set; } = string.Empty;
        public DateTime TokenExpiresAt { get; set; }

        [StringLength(200)]
        public string? PublicKey { get; set; }          // devuelto por OAuth, sirve al front

        public bool IsConnected { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Creator Creator { get; set; } = null!;
    }
}
