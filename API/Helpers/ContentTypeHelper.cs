namespace API.Helpers
{
    public static class ContentTypeHelper
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".pdf", ".txt"
        };

        public static bool IsAllowed(string ext) => AllowedExtensions.Contains(ext);

        public static string FromExtension(string ext) => ext.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png"            => "image/png",
            ".gif"            => "image/gif",
            ".webp"           => "image/webp",
            ".pdf"            => "application/pdf",
            ".txt"            => "text/plain",
            _                 => "application/octet-stream"
        };
    }
}
