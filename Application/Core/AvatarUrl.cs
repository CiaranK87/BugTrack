namespace Application.Core
{
    public static class AvatarUrl
    {
        public static string For(string username) =>
            $"/api/profiles/{username}/avatar";
    }
}
