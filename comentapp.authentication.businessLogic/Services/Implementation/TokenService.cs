

using comentapp.authentication.businessLogic.Core;
using comentapp.infrastructure.Options;
using comentapp.persistence.Models;
using comentapp.persistence.Repository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace comentapp.authentication.businessLogic.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly JwtOptions _jwtOptions;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public TokenService(JwtOptions jwtOptions, IRefreshTokenRepository refreshTokenRepository)
        {
            _jwtOptions = jwtOptions;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //new Claim(ClaimTypes.Role,               user.Role),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtOptions.AccessTokenExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken(int userId) => new()
        {
            UserId = userId,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenExpirationDays),
            IsRevoked = false
        };

        public async Task<Result<AuthTokens>> RefreshAsync(string refreshToken)
        {
            var stored = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

            if (stored is null)
                return Result<AuthTokens>.Failure("Refresh token inválido.", 401);

            if (stored.ExpiresAt < DateTime.UtcNow)
            {
                await _refreshTokenRepository.RevokeByIdAsync(stored.Id);
                return Result<AuthTokens>.Failure("Refresh token expirado.", 401);
            }

            // Rotación — el token viejo se revoca y se emite uno nuevo
            await _refreshTokenRepository.RevokeByIdAsync(stored.Id);

            var user = stored.User;
            var newAccess = GenerateAccessToken(user);
            var newRefresh = GenerateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(newRefresh);
            await _refreshTokenRepository.SaveChangesAsync();

            return Result<AuthTokens>.Success(new AuthTokens
            {
                AccessToken = newAccess,
                RefreshToken = newRefresh.Token,
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AuthProvider = "local"
            });
        }

        public async Task RevokeAsync(string refreshToken)
        {
            await _refreshTokenRepository.GetByTokenAsync(refreshToken).ContinueWith(async t =>
            {
                var token = t.Result;
                if (token is not null)
                {
                    await _refreshTokenRepository.RevokeByIdAsync(token.Id);
                    await _refreshTokenRepository.SaveChangesAsync();
                }
            });
        }

        public async Task SaveRefreshTokenAsync(RefreshToken refreshToken)
        {
            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();
        }
    }
}
