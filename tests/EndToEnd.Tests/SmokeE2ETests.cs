using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Xunit;
using System.Net.Http;
using System.Text.Json;

namespace EndToEnd.Tests
{
    public class SmokeE2ETests : BaseE2ETest
    {
        [E2EFact]
        public async Task Frontend_ShouldBeAccessible()
        {
            var response = await _httpClient.GetAsync(TestConfigurations.Urls.BaseUrl);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [E2EFact]
        public async Task API_ShouldBeAccessible()
        {
            // Health endpoint might not exist, so accept 200 or 404
            var response = await _httpClient.GetAsync($"{TestConfigurations.Urls.ApiUrl}/api/health");
            response.StatusCode.Should().BeOneOf(
                System.Net.HttpStatusCode.OK,
                System.Net.HttpStatusCode.NotFound);
        }

        [E2EFact]
        public async Task LoginEndpoint_ShouldExist()
        {
            var loginRequest = new
            {
                email = "test@test.com",
                password = "WrongPassword"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(loginRequest),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{TestConfigurations.Urls.ApiUrl}/api/account/login", content);
            response.StatusCode.Should().BeOneOf(
                System.Net.HttpStatusCode.Unauthorized,
                System.Net.HttpStatusCode.OK);
        }

        [E2EFact]
        public async Task ProtectedEndpoints_ShouldRequireAuthentication()
        {
            var response = await _httpClient.GetAsync($"{TestConfigurations.Urls.ApiUrl}/api/account");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [E2EFact]
        public async Task ProjectsEndpoint_ShouldRequireAuthentication()
        {
            var response = await _httpClient.GetAsync($"{TestConfigurations.Urls.ApiUrl}/api/projects");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [E2EFact]
        public async Task TicketsEndpoint_ShouldRequireAuthentication()
        {
            var response = await _httpClient.GetAsync($"{TestConfigurations.Urls.ApiUrl}/api/tickets");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [E2EFact]
        public async Task LoginPage_ShouldLoadInBrowser()
        {
            await _page.GotoAsync($"{TestConfigurations.Urls.BaseUrl}/login");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var title = await _page.TitleAsync();
            title.Should().NotBeNullOrEmpty();
            
            var loginForm = await _page.QuerySelectorAsync("form");
            loginForm.Should().NotBeNull();
        }

        [E2EFact]
        public async Task ProjectsPage_ShouldLoadInBrowser()
        {
            await _page.GotoAsync($"{TestConfigurations.Urls.BaseUrl}/projects");
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            var title = await _page.TitleAsync();
            title.Should().NotBeNullOrEmpty();
            
            // Page should load (might redirect to login if not authenticated)
            var content = await _page.TextContentAsync("body");
            content.Should().NotBeNullOrEmpty();
        }
    }
}