using comentapp.persistence.Models;

namespace comentapp.persistence.Repository
{
    /// <summary>
    /// Repository abstraction for <see cref="User"/> persistence operations,
    /// extending the generic <see cref="IRepository{T}"/> with user-specific lookups.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Finds a user by their unique email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The matching <see cref="User"/>, or <c>null</c> if none exists.</returns>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Finds a user by their unique username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The matching <see cref="User"/>, or <c>null</c> if none exists.</returns>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Loads a user by id, including their related <see cref="Creator"/> profile (if any).
        /// Used for hydration scenarios (e.g. current-user session data) that need to know
        /// whether the user also owns a creator profile.
        /// </summary>
        /// <param name="id">The user id.</param>
        /// <returns>The matching <see cref="User"/> with <see cref="User.Creator"/> loaded, or <c>null</c> if none exists.</returns>
        Task<User?> GetByIdWithCreatorAsync(int id);

        /// <summary>
        /// Checks whether a user with the given email already exists.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns><c>true</c> if a user with that email exists; otherwise <c>false</c>.</returns>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Checks whether a user with the given username already exists.
        /// </summary>
        /// <param name="username">The username to check.</param>
        /// <returns><c>true</c> if a user with that username exists; otherwise <c>false</c>.</returns>
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Adds a new user to the persistence context. Does not save changes.
        /// </summary>
        /// <param name="user">The user entity to create.</param>
        /// <returns>The tracked user entity.</returns>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Marks an existing user entity as modified. Does not save changes.
        /// </summary>
        /// <param name="user">The user entity to update.</param>
        Task UpdateUserAsync(User user);
    }
}
