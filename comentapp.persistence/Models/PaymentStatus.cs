using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.persistence.Models
{
    public class PaymentStatus
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public enum PaymentStatusIds
    {
        Pending = 1,
        Completed,
        Failed,
        Cancelled,
        Refunded
    }
}
