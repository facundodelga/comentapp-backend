using comentapp.persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace comentapp.persistence.Repository.Implementation
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ComentappDbContext context)
            : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(user => user.UserName == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(user => user.UserName == username);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            var i = await _context.Users.AddAsync(user);
            return i.Entity;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
        }

    }
}
