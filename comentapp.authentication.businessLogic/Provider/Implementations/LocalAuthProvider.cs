using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Services;
using Microsoft.Extensions.Logging;

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    /// <summary>
    /// <see cref="IAuthProvider"/> implementation for local (email/password) authentication.
    /// Delegates credential validation to <see cref="IUserService"/> and issues tokens via <see cref="ITokenService"/>.
    /// </summary>
    public class LocalAuthProvider(
        ILogger<LocalAuthProvider> logger,
        IUserService userService, 
        ITokenService tokenService) : IAuthProvider
    {
        /// <inheritdoc />
        public string ProviderName => "local";

        private readonly ILogger<LocalAuthProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        /// <inheritdoc />
        public async Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var userServiceResult = await _userService.LoginUser(request);

            if (!userServiceResult.IsSuccess)
            {
                // IUserService.LoginUser already rejects unconfirmed emails, so no
                // additional confirmation check is needed here.
                return Result<AuthTokens>.Failure(userServiceResult.ErrorMessage, userServiceResult.ErrorCode ?? 0);
            }

            var user = userServiceResult.Value;
            var token = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            _logger.LogInformation("User {Email} authenticated successfully", user.Email);

            // Guardar el refresh token en la BD
            await _tokenService.SaveRefreshTokenAsync(refreshToken);

            return Result<AuthTokens>.Success(new AuthTokens
            {
                AccessToken = token,
                RefreshToken = refreshToken.Token,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AuthProvider = "local"
            });
        }
    }
}
