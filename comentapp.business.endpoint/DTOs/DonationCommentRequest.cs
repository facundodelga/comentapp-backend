using System.ComponentModel.DataAnnotations;

namespace comentapp.business.endpoint.DTOs
{
    public class DonationCommentRequest
    {
        [Required]
        public int CreatorId { get; set; }

        [Required]
        [StringLength(300, MinimumLength = 1)]
        public string Comment { get; set; } = string.Empty;

        [Range(typeof(decimal), "0.01", "9999999", ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Amount { get; set; }
    }
}
