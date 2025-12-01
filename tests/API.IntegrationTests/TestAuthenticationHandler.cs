using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace API.IntegrationTests
{
    public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Check for our custom test authentication header
            if (Request.Headers.TryGetValue("Authorization", out var authValue) &&
                authValue.ToString().StartsWith("Test "))
            {
                var authData = authValue.ToString().Substring(5); // Remove "Test " prefix
                
                // Parse additional auth data if provided in JSON format
                Dictionary<string, string> authParams = null;
                if (!string.IsNullOrEmpty(authData) && authData.StartsWith("{"))
                {
                    try
                    {
                        authParams = JsonSerializer.Deserialize<Dictionary<string, string>>(authData);
                    }
                    catch
                    {
                        // If parsing fails, treat authData as just the role
                        authParams = new Dictionary<string, string> { ["globalrole"] = authData };
                    }
                }
                else
                {
                    // Simple format: just the role
                    authParams = new Dictionary<string, string> { ["globalrole"] = authData };
                }
                
                var userId = authParams?.GetValueOrDefault("userId") ?? "test-user-id";
                var userName = authParams?.GetValueOrDefault("userName") ?? "testuser";
                var userEmail = authParams?.GetValueOrDefault("userEmail") ?? "test@example.com";
                var globalRole = authParams?.GetValueOrDefault("globalrole") ?? "Admin";
                
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, userEmail),
                    new Claim("globalrole", globalRole)
                };

                // Add project role claims if provided
                if (authParams != null)
                {
                    foreach (var kvp in authParams)
                    {
                        if (kvp.Key.StartsWith("project_"))
                        {
                            claims.Add(new Claim(kvp.Key, kvp.Value));
                        }
                    }
                }

                var identity = new ClaimsIdentity(claims, "Test");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Test");
                
                // Set the HttpContext User for the rest of the request pipeline
                Context.User = principal;
                
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
        }
    }
}