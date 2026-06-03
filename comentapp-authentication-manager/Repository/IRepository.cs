using System.Linq.Expressions;

namespace comentapp_authentication_manager.Repository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        Task SaveChangesAsync();
    }
}
