using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Cross_platform_development_technologies.Models;
using Microsoft.IdentityModel.Tokens;

namespace Cross_platform_development_technologies.Services
{
    public class TokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateToken(AppUser user)
        {
            var issuer = _configuration["Jwt:Issuer"]
                ?? throw new InvalidOperationException(
                    "Не задан Jwt:Issuer.");

            var audience = _configuration["Jwt:Audience"]
                ?? throw new InvalidOperationException(
                    "Не задан Jwt:Audience.");

            var signingKey = _configuration["Jwt:SigningKey"]
                ?? throw new InvalidOperationException(
                    "Не задан Jwt:SigningKey.");

            if (!int.TryParse(
                    _configuration["Jwt:LifetimeMinutes"],
                    out int lifetimeMinutes)
                || lifetimeMinutes <= 0)
            {
                throw new InvalidOperationException(
                    "Jwt:LifetimeMinutes должен быть положительным целым числом.");
            }

            var keyBytes = Convert.FromBase64String(signingKey);

            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString(CultureInfo.InvariantCulture)),

                new Claim("name", user.Login),

                new Claim("role", user.Role),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };

            var now = DateTime.UtcNow;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(lifetimeMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}