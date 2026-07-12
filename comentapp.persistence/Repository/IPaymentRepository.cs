using comentapp.persistence.Models;

namespace comentapp.persistence.Repository
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<Payment?> GetByExternalReferenceAsync(string externalReference);
    }
}
