using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using System.Net.Http;

namespace EndToEnd.Tests
{
    public abstract class BaseE2ETest : IAsyncLifetime
    {
        protected IPlaywright? _playwright;
        protected IBrowser? _browser;
        protected IPage? _page;
        protected readonly HttpClient _httpClient = new();

        public async Task InitializeAsync()
        {
            try
            {
                _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    SlowMo = 0,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
                });
                _page = await _browser.NewPageAsync();
            }
            catch (Exception ex)
            {
                // If Playwright fails to initialize, try with more basic settings
                try
                {
                    if (_playwright == null)
                        _playwright = await Microsoft.Playwright.Playwright.CreateAsync();
                    
                    if (_browser != null)
                        await _browser.CloseAsync();
                        
                    _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                    {
                        Headless = true
                    });
                    _page = await _browser.NewPageAsync();
                }
                catch (Exception innerEx)
                {
                    // If all attempts fail, rethrow the original exception
                    throw new Exception($"Failed to initialize Playwright: {ex.Message}. Inner exception: {innerEx.Message}");
                }
            }
        }

        public async Task DisposeAsync()
        {
            if (_page != null)
                await _page.CloseAsync();
            if (_browser != null)
                await _browser.CloseAsync();
            if (_playwright != null)
                _playwright.Dispose();
            _httpClient.Dispose();
        }
    }
}