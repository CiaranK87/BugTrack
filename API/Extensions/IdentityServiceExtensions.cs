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
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<DataContext>()
            .AddSignInManager<SignInManager<AppUser>>()
            .AddRoleManager<RoleManager<IdentityRole>>();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                config.GetEnvironmentVariable("TOKEN_KEY") ??
                config["TokenKey"] ??
                "super secret key that needs to be at least 16 characters"));

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
