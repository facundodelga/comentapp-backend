namespace comentapp.authentication.businessLogic.Core
{
    /// <summary>
    /// Error codes returned via <see cref="Result{T}.ErrorCode"/> by <see cref="Services.IUserService"/> operations.
    /// </summary>
    public enum UserServiceErrorCodes
    {
        /// <summary>Registration failed because the email is already registered.</summary>
        CU_EmailAlreadyExists,

        /// <summary>Registration failed because the username is already taken.</summary>
        CU_UsernameAlreadyExists,

        /// <summary>
        /// Generic "user not found / invalid credentials / not confirmed" code, used for
        /// login failures and lookups so as not to leak which specific check failed.
        /// </summary>
        LU_UserNotFound,
    }
}
