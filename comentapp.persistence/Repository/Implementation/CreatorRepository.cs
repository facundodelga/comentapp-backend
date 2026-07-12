using comentapp.persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace comentapp.persistence.Repository.Implementation
{
    public class CreatorRepository(ComentappDbContext context)
        : Repository<Creator>(context), ICreatorRepository
    {
        public async Task<Creator?> GetByUserIdAsync(int userId)
        {
            return await _context.Creators.FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
