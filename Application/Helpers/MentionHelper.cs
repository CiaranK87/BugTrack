using System.Text.RegularExpressions;

namespace Application.Helpers
{
    public static class MentionHelper
    {
        // Match @username where username can contain letters, digits, -, ., _, @, +, '
        // Must be preceded by start of string or whitespace to avoid matching emails.
        private static readonly Regex MentionRegex = new Regex(@"(?<=^|\s)@([a-zA-Z0-9\-._@+']+)", RegexOptions.Compiled);

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
