namespace comentapp_authentication_manager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsEmailConfirmed { get; set; } = false;
        public DateTime LastModifiedDate { get; set; }
    }
}
