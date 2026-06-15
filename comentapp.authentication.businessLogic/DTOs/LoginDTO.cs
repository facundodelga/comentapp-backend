using comentapp.persistence.Models;
namespace comentapp.authentication.businessLogic.DTOs
{
    public class LoginDTO
    {
        public User User { get; set; } = new User();
    }
}
