using comentapp.persistence.Models;

namespace comentapp.persistence.Repository
{
    public interface IMercadoPagoOAuthStateRepository : IRepository<MercadoPagoOAuthState>
    {
        Task<MercadoPagoOAuthState?> GetByStateAsync(string state);
    }
}
