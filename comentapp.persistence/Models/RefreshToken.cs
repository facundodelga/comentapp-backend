using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.persistence.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRevoked { get; set; } = false;

        // Navigation
        public User User { get; set; } = null!;
    }
}
