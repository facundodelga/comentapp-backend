namespace comentapp.business.endpoint.DTOs
{
    public class CommentRequest
    {
        public string CommentText { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
    }
}
