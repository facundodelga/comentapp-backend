using comentapp.persistence.Models;

namespace comentapp.persistence.Repository
{
    public interface ICreatorMercadoPagoAccountRepository : IRepository<CreatorMercadoPagoAccount>
    {
        Task<CreatorMercadoPagoAccount?> GetByCreatorIdAsync(int creatorId);
    }
}
