using comentapp.persistence.Models;

namespace comentapp.authentication.businessLogic.DTOs
{
    public class RegisterDTO
    {
        public User User { get; set; } = new User();
    }
}