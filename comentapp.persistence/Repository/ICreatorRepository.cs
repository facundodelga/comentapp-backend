using comentapp.persistence.Models;

namespace comentapp.persistence.Repository
{
    public interface ICreatorRepository : IRepository<Creator>
    {
        Task<Creator?> GetByUserIdAsync(int userId);
    }
}
