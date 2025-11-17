namespace API.Extensions
{
    public static class EnvironmentExtensions
    {
        public static string GetEnvironmentVariable(this IConfiguration configuration, string name)
        {
            // First try environment variable, then configuration
            var envValue = Environment.GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(envValue))
                return envValue;
            
            return configuration[name];
        }
    }
}