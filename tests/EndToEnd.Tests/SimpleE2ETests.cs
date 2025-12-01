using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;

namespace EndToEnd.Tests
{
    public class SimpleE2ETests : BaseE2ETest
    {
        [Fact]
        public async Task HomePage_ShouldLoad()
        {
            await _page.GotoAsync(TestConfigurations.Urls.BaseUrl);
            var title = await _page.TitleAsync();
            title.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ProjectsPage_ShouldBeAccessible()
        {
            await _page.GotoAsync($"{TestConfigurations.Urls.BaseUrl}/projects");
            var title = await _page.TitleAsync();
            title.Should().NotBeNullOrEmpty();
            
            var content = await _page.TextContentAsync("body");
            content.Should().NotBeNullOrEmpty();
        }
    }
}