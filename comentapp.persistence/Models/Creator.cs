

using System.ComponentModel.DataAnnotations;

namespace comentapp.persistence.Models
{
    public class Creator
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string CreatorName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string MercadoPagoAccount { get; set; } = string.Empty;
        [StringLength(300)]
        public string? InstagramLink { get; set; }
        [StringLength(300)]
        public string? TikTokLink { get; set; }
        [StringLength(300)]
        public string? YouTubeLink { get; set; }
        [StringLength(300)]
        public string? TwitchLink { get; set; }
        [StringLength(300)]
        public string? KickLink { get; set; }
        [StringLength(1000)]
        public string? Description { get; set; }


        public User User { get; set; } = null!;
        public ICollection<Payment> Payments { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
