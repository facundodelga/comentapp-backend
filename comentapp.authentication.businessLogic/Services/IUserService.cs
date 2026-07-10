using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.persistence.Models;

namespace comentapp.authentication.businessLogic.Services
{
    /// <summary>
    /// Application service for user account operations: registration, login,
    /// email confirmation, and current-user hydration.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user, sending an email-confirmation link on success.
        /// </summary>
        /// <param name="register">The registration data, including the desired user fields.</param>
        /// <returns>
        /// A successful <see cref="Result{T}"/> containing the created <see cref="User"/>,
        /// or a failure result with <see cref="UserServiceErrorCodes.CU_EmailAlreadyExists"/>/
        /// <see cref="UserServiceErrorCodes.CU_UsernameAlreadyExists"/> when the email or username is taken.
        /// </returns>
        Task<Result<User>> RegisterUser(RegisterDTO register);

        /// <summary>
        /// Validates email/password credentials for local login.
        /// Fails when the user does not exist, the password is incorrect,
        /// or the user has not confirmed their email.
        /// </summary>
        /// <param name="login">The login credentials.</param>
        /// <returns>A successful <see cref="Result{T}"/> containing the authenticated <see cref="User"/>, or a failure result.</returns>
        Task<Result<User>> LoginUser(LoginDTO login);

        /// <summary>
        /// Confirms a user's email address using a previously issued confirmation token.
        /// </summary>
        /// <param name="confirmEmail">The email and token to validate.</param>
        /// <returns>A successful <see cref="Result{T}"/> containing the confirmed <see cref="User"/>, or a failure result.</returns>
        Task<Result<User>> ConfirmEmailAsync(ConfirmMailDTO confirmEmail);

        /// <summary>
        /// Loads the current user by id, including their creator profile (if any),
        /// for session hydration (e.g. the <c>GET /Authentication/me</c> endpoint).
        /// </summary>
        /// <param name="userId">The id of the authenticated user, typically resolved from <see cref="System.Security.Claims.ClaimTypes.NameIdentifier"/>.</param>
        /// <returns>A successful <see cref="Result{T}"/> containing the <see cref="User"/> with <see cref="User.Creator"/> loaded, or a failure result if the user no longer exists.</returns>
        Task<Result<User>> GetCurrentUserAsync(int userId);

        /// <summary>
        /// Finds the user matching a verified Google email, or creates a new user from the
        /// Google profile claims when no account exists yet. Email is the account key: the
        /// same email never results in two different users, whether they registered locally
        /// or through Google.
        /// </summary>
        /// <param name="email">The verified email address returned by Google.</param>
        /// <param name="name">The given name from the Google profile, used only when creating a new user.</param>
        /// <param name="surname">The family name from the Google profile, used only when creating a new user.</param>
        /// <returns>
        /// A successful <see cref="Result{T}"/> containing the existing or newly created <see cref="User"/>.
        /// Newly created users are marked with <see cref="User.IsEmailConfirmed"/> = <c>true</c> and do not
        /// receive a usable password (Google is their only sign-in method).
        /// </returns>
        Task<Result<User>> FindOrCreateGoogleUserAsync(string email, string? name, string? surname, string? username);
    }
}
