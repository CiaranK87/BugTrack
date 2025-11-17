using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using API.Extensions;

namespace API.Services
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration config, DataContext context, UserManager<AppUser> userManager)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new("globalrole", user.GlobalRole ?? "User"),
            };

            var tokenKey = _config.GetEnvironmentVariable("TOKEN_KEY") ??
                          _config["TokenKey"] ??
                          "super secret key that needs to be at least 16 characters";
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Configure token expiry based on environment
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

        public string CreateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
