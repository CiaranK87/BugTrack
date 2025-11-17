using Application.Core;
using Application.Interfaces;
using Application.Projects;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Npgsql;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddDbContext<DataContext>(opt =>
            {
                // Try to get connection string from environment variable first, then from configuration
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                    ?? config.GetConnectionString("DefaultConnection");
                
                // Use PostgreSQL for all environments
                opt.UseNpgsql(connectionString);
            });

            services.AddCors(opt => {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    var allowedOrigins = config["ALLOWED_ORIGINS"]?.Split(',')
                        ?? new[] { "http://localhost:3000" };
                    
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .WithOrigins(allowedOrigins)
                          .AllowCredentials();
                });
            });

            // Configure SignalR
            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = false;
            }).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(List.Handler).Assembly));
            services.AddAutoMapper(typeof(MappingProfiles).Assembly);
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<Create>(); 
            services.AddHttpContextAccessor();
            services.AddScoped<IUserAccessor, UserAccessor>();

            return services;
        }
    }
}