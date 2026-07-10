using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Services;
using Microsoft.Extensions.Logging;

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    /// <summary>
    /// <see cref="IAuthProvider"/> implementation for Google OAuth sign-in.
    /// Called by <c>AuthenticationController.GoogleCallback</c> after the external
    /// (Google) principal has been read from the <c>ExternalCookie</c> scheme. Finds or
    /// creates the local user from the verified Google email and issues the same
    /// session tokens as <see cref="LocalAuthProvider"/>, tagged with <c>AuthProvider = "google"</c>.
    /// </summary>
    public class GoogleAuthProvider(
        ILogger<GoogleAuthProvider> logger,
        IUserService userService,
        ITokenService tokenService) : IAuthProvider
    {
        /// <inheritdoc />
        public string ProviderName => "google";

        private readonly ILogger<GoogleAuthProvider> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IUserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        private readonly ITokenService _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));

        /// <summary>
        /// Authenticates a Google identity, expressed through <paramref name="request"/>.User
        /// (Email/Name/Surname populated from the verified Google claims by the controller).
        /// </summary>
        /// <inheritdoc />
        public async Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var googleProfile = request.User;
            _logger.LogInformation("User {Email} search", googleProfile.Email);
            if (string.IsNullOrWhiteSpace(googleProfile.Email))
            {
                return Result<AuthTokens>.Failure(
                    "No se pudo obtener un email verificado desde Google.",
                    (int)UserServiceErrorCodes.LU_UserNotFound);
            }

            var userResult = await _userService.FindOrCreateGoogleUserAsync(
                googleProfile.Email,
                googleProfile.Name,
                googleProfile.Surname, 
                googleProfile.UserName);

            if (!userResult.IsSuccess)
                return Result<AuthTokens>.Failure(userResult.ErrorMessage, userResult.ErrorCode ?? 0);

            var user = userResult.Value;
            var token = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            _logger.LogInformation("User {Email} authenticated successfully via Google", user.Email);

            await _tokenService.SaveRefreshTokenAsync(refreshToken);

            return Result<AuthTokens>.Success(new AuthTokens
            {
                AccessToken = token,
                RefreshToken = refreshToken.Token,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AuthProvider = "google"
            });
        }
    }
}
