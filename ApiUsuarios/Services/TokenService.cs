using ApiUsuarios.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ApiUsuarios.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(Usuario usuario)
        {
            return BuildToken(usuario, isRefresh: false);
        }

        public string GenerateRefreshToken(Usuario usuario)
        {
            return BuildToken(usuario, isRefresh: true);
        }

        public ClaimsPrincipal? GetPrincipalFromRefreshToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidAudience = _config["Jwt:Audience"],
                IssuerSigningKey = key
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(token, validationParams, out _);

                var isRefreshClaim = principal.FindFirstValue("isRefresh");
                if (isRefreshClaim != "true")
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public static string HashPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = SHA256.HashData(bytes);
            return Convert.ToHexString(hash).ToLower();
        }

        private string BuildToken(Usuario usuario, bool isRefresh)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, usuario.NombreUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("isRefresh", isRefresh ? "true" : "false")
            };

            int expirationMinutes = isRefresh
                ? int.Parse(_config["Jwt:RefreshExpirationMinutes"]!)
                : int.Parse(_config["Jwt:ExpirationMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
