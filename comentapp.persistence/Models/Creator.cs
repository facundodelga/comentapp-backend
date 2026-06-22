

namespace comentapp.persistence.Models
{
    public class Creator
    {
        public int Id { get; set; }
        public string CreatorName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string MercadoPagoAccount { get; set; } = string.Empty;
        public string? InstagramLink { get; set; }
        public string? TikTokLink { get; set; }
        public string? YouTubeLink { get; set; }
        public string? TwitchLink { get; set; }
        public string? KickLink { get; set; }
        public string? Description { get; set; }
        public User User { get; set; } = null!;

        public List<Comment> Comments { get; set; } = new List<Comment>();
    }
}
