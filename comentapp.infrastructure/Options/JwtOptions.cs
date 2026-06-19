using System;
using System.Collections.Generic;
using System.Text;

namespace comentapp.infrastructure.Options
{
    public class JwtOptions
    {
        public const string Section = "Jwt";

        public string Secret { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int AccessTokenExpirationMinutes { get; init; } = 15;
        public int RefreshTokenExpirationDays { get; init; } = 7;
    }
}
