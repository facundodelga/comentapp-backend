

using System.ComponentModel.DataAnnotations;

namespace comentapp.persistence.Models
{
    public class Comment
    {
        public int Id { get; set; }
        [StringLength(300)]
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public int CreatorId { get; set; }
        public int PaymentId { get; set; }
        public User User { get; set; } = null!;
        public Creator Creator { get; set; } = null!;

    }
}


