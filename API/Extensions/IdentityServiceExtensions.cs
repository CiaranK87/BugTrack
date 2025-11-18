using API.Services;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System.Text;

namespace API.Extensions
{
    public static class IdentityServiceExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = true;
                opt.User.RequireUniqueEmail = true;
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleManager<RoleManager<IdentityRole>>();

            var tokenKey = config.GetEnvironmentVariable("TOKEN_KEY") ??
                          config["TokenKey"] ??
                          throw new InvalidOperationException("TOKEN_KEY environment variable is not configured");
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            // Configure JWT settings based on environment
            var isDevelopment = config.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            var issuer = config.GetEnvironmentVariable("JWT_ISSUER") ?? "http://localhost:5000";
            var audience = config.GetEnvironmentVariable("JWT_AUDIENCE") ?? "http://localhost:5000";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        ClockSkew = TimeSpan.Zero
                    };
                    
                    // Production-specific settings
                    if (!isDevelopment)
                    {
                        opt.RequireHttpsMetadata = true;
                        opt.SaveToken = true;
                    }
                });

            services.AddScoped<TokenService>();

            return services;
        }
    }
}
