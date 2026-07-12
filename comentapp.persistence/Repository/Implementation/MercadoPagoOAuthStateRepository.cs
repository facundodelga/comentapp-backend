using comentapp.persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace comentapp.persistence.Repository.Implementation
{
    public class MercadoPagoOAuthStateRepository(ComentappDbContext context)
        : Repository<MercadoPagoOAuthState>(context), IMercadoPagoOAuthStateRepository
    {
        public async Task<MercadoPagoOAuthState?> GetByStateAsync(string state)
        {
            return await _context.MercadoPagoOAuthStates
                .FirstOrDefaultAsync(s => s.State == state);
        }
    }
}
