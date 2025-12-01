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
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                    ?? config.GetConnectionString("DefaultConnection");
                
                // Use PostgreSQL for all environments
                opt.UseNpgsql(connectionString);
            });

            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    var env = config["ASPNETCORE_ENVIRONMENT"];
                    var isDevelopment = env == "Development";

                    var rawOrigins = config["ALLOWED_ORIGINS"];

                    var configuredOrigins = rawOrigins?
                        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        ?? Array.Empty<string>();

                    string[] devOrigins = Array.Empty<string>();

                    if (isDevelopment)
                    {
                        devOrigins = new[]
                        {
                            "http://localhost:3000",
                            "https://localhost:3000"
                        };
                    }

                    // Merge + remove duplicates
                    var finalOrigins = configuredOrigins
                        .Concat(devOrigins)
                        .Distinct()
                        .ToArray();

                    if (!finalOrigins.Any())
                        throw new Exception("No CORS origins configured! Add ALLOWED_ORIGINS to settings.");

                    policy.WithOrigins(finalOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
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