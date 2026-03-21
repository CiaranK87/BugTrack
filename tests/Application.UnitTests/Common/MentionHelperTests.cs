using Application.Helpers;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Common
{
    public class MentionHelperTests
    {
        [Theory]
        [InlineData("Hello @alice", new[] { "alice" })]
        [InlineData("Hello @alice, how are you?", new[] { "alice" })]
        [InlineData("Hey @MikeOBrien and @Bob!", new[] { "MikeOBrien", "Bob" })]
        [InlineData("Check this @DenisO'Brien", new[] { "DenisO'Brien" })]
        [InlineData("No mention here", new string[0])]
        [InlineData("Email at test@example.com", new string[0])]
        [InlineData("@starting with mention", new[] { "starting" })]
        [InlineData("Multi @word-name.test_123", new[] { "word-name.test_123" })]
        public void ExtractMentions_ShouldExtractCorrectUsernames(string content, string[] expected)
        {
            // Act
            var result = MentionHelper.ExtractMentions(content);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
