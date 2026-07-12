using System;
using System.ComponentModel.DataAnnotations;

namespace comentapp.persistence.Models
{
    public class MercadoPagoOAuthState
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string State { get; set; } = string.Empty;   // random, anti-CSRF

        public int CreatorId { get; set; }       // quién inició
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }  // corto, ej 10 min
        public bool Used { get; set; }

        public Creator Creator { get; set; } = null!;
    }
}
