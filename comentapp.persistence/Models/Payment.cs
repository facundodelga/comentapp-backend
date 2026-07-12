using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace comentapp.persistence.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string Method { get; set; } = "Mercado Pago";
        public int PaymentStatusId { get; set; } = (int)PaymentStatusIds.Pending;
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PayedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string MercadoPagoId { get; set; }
        public string PreferenceId { get; set; }

        public string? CollectorId { get; set; }     // cuenta del creador que cobró
        [Column(TypeName = "decimal(18,2)")]
        public decimal MarketplaceFee { get; set; }  // comisión app (3%)
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetReceivedAmount { get; set; }// lo que queda al creador
        public string? MpRawStatus { get; set; }     // status crudo MP (approved/pending/...)
        public string? StatusDetail { get; set; }    // status_detail MP

        public int CreatorId { get; set; }
        public int UserId { get; set; }
        public int CommentId { get; set; }
        public User User { get; set; }
        public Creator Creator { get; set; }
        public Comment Comment { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
    }

   
}
