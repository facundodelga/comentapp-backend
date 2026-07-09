using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.persistence.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Method { get; set; } = "Mercado Pago";
        public int PaymentStatusId { get; set; } = (int)PaymentStatusIds.Pending;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PayedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string MercadoPagoId { get; set; }
        public string PreferenceId { get; set; }

        public int CreatorId { get; set; }
        public int UserId { get; set; }
        public int CommentId { get; set; }
        public User User { get; set; }
        public Creator Creator { get; set; }
        public Comment Comment { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
    }

   
}
