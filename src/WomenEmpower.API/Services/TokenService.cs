using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WomenEmpower.Core.Entities;

namespace WomenEmpower.API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(ApplicationUser user) // Fixed spelling: Token
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(ClaimTypes.GivenName, user.OrganizationName ?? "")
            };

            // FIX: Match these keys exactly to your appsettings.json
            var secretKey = _config["JwtSettings:SecretKey"]; // Was "SecretKet"
            if (string.IsNullOrEmpty(secretKey)) throw new Exception("SecretKey is missing!");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            // FIX: Match duration key
            var duration = _config["JwtSettings:DurationInMinutes"]; // Was "JWTSettings"
            var expiryMinutes = double.TryParse(duration, out var result) ? result : 60;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(expiryMinutes),
                SigningCredentials = credentials,
                Issuer = _config["JwtSettings:Issuer"],   // Fixed: Issuer
                Audience = _config["JwtSettings:Audience"] // Fixed: Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}