using comentapp.persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace comentapp.persistence.Repository.Implementation
{
    public class PaymentRepository(ComentappDbContext context)
        : Repository<Payment>(context), IPaymentRepository
    {
        // Se usa el id del Payment como external_reference; la conciliación del webhook
        // lo busca por ese valor.
        public async Task<Payment?> GetByExternalReferenceAsync(string externalReference)
        {
            if (!int.TryParse(externalReference, out var id))
            {
                return null;
            }

            return await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
