using comentapp.persistence.Models;

namespace comentapp.persistence.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);

        Task<User?> GetByUsernameAsync(string username);

        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
    }
}
