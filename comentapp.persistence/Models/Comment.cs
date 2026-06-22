using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.persistence.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public int CreatorId { get; set; }

        public User User { get; set; } = null!;
        public Creator Creator { get; set; } = null!;

    }
}
