using comentapp.persistence.Models;
namespace comentapp.authentication.businessLogic.DTOs
{
    public class LoginDTO
    {
        public User User { get; set; } = new User();

        // OAuth (Google, etc.)
        public string? IdToken { get; init; }
        public string? Provider { get; init; }
    }
}
