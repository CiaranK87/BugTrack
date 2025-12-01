namespace EndToEnd.Tests
{
    public static class TestConfigurations
    {
        public static class Urls
        {
            public static string BaseUrl => Environment.GetEnvironmentVariable("E2E_BASE_URL") ?? "http://localhost:3000";
            public static string ApiUrl => Environment.GetEnvironmentVariable("E2E_API_URL") ?? "http://localhost:5000";
        }

        public static class Users
        {
            public static readonly User Admin = new()
            {
                Email = Environment.GetEnvironmentVariable("E2E_ADMIN_EMAIL") ?? "admin@test.com",
                Password = Environment.GetEnvironmentVariable("E2E_ADMIN_PASSWORD") ?? "TestPassword123!"
            };
            
            public static readonly User User = new()
            {
                Email = Environment.GetEnvironmentVariable("E2E_USER_EMAIL") ?? "user@test.com",
                Password = Environment.GetEnvironmentVariable("E2E_USER_PASSWORD") ?? "TestPassword123!"
            };
            
            public static readonly User ProjectManager = new()
            {
                Email = Environment.GetEnvironmentVariable("E2E_PM_EMAIL") ?? "pm@test.com",
                Password = Environment.GetEnvironmentVariable("E2E_PM_PASSWORD") ?? "TestPassword123!"
            };
        }
    }

    public class User
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}