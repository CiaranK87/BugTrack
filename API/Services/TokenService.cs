using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.IdentityModel.Tokens;
using API.Extensions;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new("globalrole", user.GlobalRole ?? Roles.Global.User),
            };

            var tokenKey = _config.GetEnvironmentVariable("TOKEN_KEY") ??
                          _config["TokenKey"] ??
                          throw new InvalidOperationException("TOKEN_KEY environment variable is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var isDevelopment = _config.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            var expiryTime = isDevelopment ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1);

            var issuer = _config.GetEnvironmentVariable("JWT_ISSUER") ?? "http://localhost:5000";
            var audience = _config.GetEnvironmentVariable("JWT_AUDIENCE") ?? "http://localhost:5000";

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiryTime,
                SigningCredentials = creds,
                Issuer = issuer,
                Audience = audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
