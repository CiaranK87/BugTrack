using System.Text.RegularExpressions;

namespace Application.Helpers
{
    public static class MentionHelper
    {
        // Match @username where username can contain letters, digits, -, ., _, @, +
        // The username must be immediately after @ and ends at whitespace or end of string
        private static readonly Regex MentionRegex = new Regex(@"@([a-zA-Z0-9\-._@+]+)", RegexOptions.Compiled);

        public static List<string> ExtractMentions(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new List<string>();

            var mentions = new List<string>();
            var matches = MentionRegex.Matches(content);

            foreach (Match match in matches)
            {
                var username = match.Groups[1].Value;
                if (!string.IsNullOrWhiteSpace(username) && !mentions.Contains(username))
                {
                    mentions.Add(username);
                }
            }

            return mentions;
        }
    }
}
