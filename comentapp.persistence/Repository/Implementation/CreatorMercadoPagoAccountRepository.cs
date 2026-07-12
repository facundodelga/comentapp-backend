using comentapp.persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace comentapp.persistence.Repository.Implementation
{
    public class CreatorMercadoPagoAccountRepository(ComentappDbContext context)
        : Repository<CreatorMercadoPagoAccount>(context), ICreatorMercadoPagoAccountRepository
    {
        public async Task<CreatorMercadoPagoAccount?> GetByCreatorIdAsync(int creatorId)
        {
            return await _context.CreatorMercadoPagoAccounts
                .FirstOrDefaultAsync(a => a.CreatorId == creatorId);
        }
    }
}
