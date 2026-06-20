using comentapp.authentication.businessLogic.Core;
using comentapp.authentication.businessLogic.DTOs;
using comentapp.authentication.businessLogic.Services;

namespace comentapp.authentication.businessLogic.Provider.Implementations
{
    public class LocalAuthProvider(IUserService userService, ITokenService tokenService) : IAuthProvider
    {
        public string ProviderName => "local";

        private readonly IUserService _userService = userService;
        private readonly ITokenService _tokenService = tokenService;

        public async Task<Result<AuthTokens>> AuthenticateAsync(LoginDTO request)
        {
            var userServiceResult = await _userService.LoginUser(request);

            if(!userServiceResult.IsSuccess)
            {
                return Result<AuthTokens>.Failure(userServiceResult.ErrorMessage, userServiceResult.ErrorCode ?? 0);
            }

            var tokens = _tokenService.GenerateAccessToken(userServiceResult.Value);

            return Result<AuthTokens>.Success(new AuthTokens
            {
                AccessToken = "userServiceResult.Value.AccessToken,",
                RefreshToken = "userServiceResult.Value.RefreshToken"
            });
        }
    }
}
