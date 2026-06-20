using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Services;
using Microsoft.Extensions.Logging;

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    public class LocalAuthProvider(
        ILogger<LocalAuthProvider> logger,
        IUserService userService, 
        ITokenService tokenService) : IAuthProvider
    {
        public string ProviderName => "local";

        private readonly ILogger<LocalAuthProvider> _logger = logger;
        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request)
        {
            var userServiceResult = await _userService.LoginUser(request);

            if (!userServiceResult.IsSuccess)
            {
                return Result<AuthTokens>.Failure(userServiceResult.ErrorMessage, userServiceResult.ErrorCode ?? 0);
            }

            var user = userServiceResult.Value;
            if (!user.IsEmailConfirmed)
            {
                _logger.LogWarning("User {Email} attempted to login with unconfirmed email", user.Email);
                return Result<AuthTokens>.Failure("Email no confirmado", (int)UserServiceErrorCodes.LU_UserNotFound);
            }

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
