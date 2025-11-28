using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using FluentAssertions;

namespace EndToEnd.Tests
{
    [TestFixture]
    public class BugManagementE2ETests : PageTest
    {
        private string _baseUrl = "http://localhost:3000";
        private string _apiUrl = "https://your-api-url.com";

        [SetUp]
        public async Task Setup()
        {
            await Page.GotoAsync(_baseUrl);
        }

        [Test]
        public async Task UserCanCreateAndManageTicket_CompleteWorkflow()
        {
            await LoginUser("user@example.com", "TestPassword123!");

            await Page.ClickAsync("[data-testid='nav-projects']");
            await Page.WaitForSelectorAsync("[data-testid='projects-list']");

            await Page.ClickAsync("[data-testid='create-project-btn']");
            await Page.FillAsync("[data-testid='project-title']", "Test Project for Bug Management");
            await Page.FillAsync("[data-testid='project-description']", "This is a test project created by E2E tests");
            await Page.ClickAsync("[data-testid='save-project-btn']");

            await Page.WaitForSelectorAsync("text=Test Project for Bug Management");
            var projectTitle = await Page.TextContentAsync("text=Test Project for Bug Management");
            projectTitle.Should().Contain("Test Project for Bug Management");

            await Page.ClickAsync("[data-testid='nav-tickets']");
            await Page.WaitForSelectorAsync("[data-testid='tickets-list']");

            await Page.ClickAsync("[data-testid='create-ticket-btn']");
            await Page.FillAsync("[data-testid='ticket-title']", "Critical Login Bug");
            await Page.FillAsync("[data-testid='ticket-description']", "Users cannot log in when using special characters in password");
            await Page.SelectOptionAsync("[data-testid='ticket-priority']", "High");
            await Page.SelectOptionAsync("[data-testid='ticket-severity']", "Critical");
            await Page.SelectOptionAsync("[data-testid='ticket-project']", "Test Project for Bug Management");
            await Page.ClickAsync("[data-testid='save-ticket-btn']");

            await Page.WaitForSelectorAsync("text=Critical Login Bug");
            var ticketTitle = await Page.TextContentAsync("text=Critical Login Bug");
            ticketTitle.Should().Contain("Critical Login Bug");

            await Page.ClickAsync("[data-testid='edit-ticket-btn']");
            await Page.FillAsync("[data-testid='ticket-title']", "Critical Login Bug - Updated");
            await Page.SelectOptionAsync("[data-testid='ticket-status']", "In Progress");
            await Page.ClickAsync("[data-testid='update-ticket-btn']");

            await Page.WaitForSelectorAsync("text=Critical Login Bug - Updated");
            var updatedTitle = await Page.TextContentAsync("text=Critical Login Bug - Updated");
            updatedTitle.Should().Contain("Critical Login Bug - Updated");

            await Page.ClickAsync("[data-testid='ticket-details-btn']");
            await Page.WaitForSelectorAsync("[data-testid='ticket-comments']");
            await Page.FillAsync("[data-testid='comment-input']", "Working on fixing the authentication issue");
            await Page.ClickAsync("[data-testid='add-comment-btn']");

            await Page.WaitForSelectorAsync("text=Working on fixing the authentication issue");
            var commentText = await Page.TextContentAsync("text=Working on fixing the authentication issue");
            commentText.Should().Contain("Working on fixing the authentication issue");

            await Page.ClickAsync("[data-testid='edit-ticket-btn']");
            await Page.SelectOptionAsync("[data-testid='ticket-status']", "Resolved");
            await Page.ClickAsync("[data-testid='update-ticket-btn']");

            await Page.WaitForSelectorAsync("[data-testid='status-resolved']");
            var statusElement = await Page.QuerySelectorAsync("[data-testid='status-resolved']");
            statusElement.Should().NotBeNull();

            await Page.SelectOptionAsync("[data-testid='status-filter']", "Resolved");
            await Page.WaitForSelectorAsync("text=Critical Login Bug - Updated");

            var resolvedTickets = await Page.QuerySelectorAllAsync("[data-testid='ticket-item']");
            resolvedTickets.Should().HaveCountGreaterOrEqualTo(1);
        }

        [Test]
        public async Task UserCanSearchAndFilterTickets()
        {
            await LoginUser("user@example.com", "TestPassword123!");
            await Page.ClickAsync("[data-testid='nav-tickets']");
            await Page.WaitForSelectorAsync("[data-testid='tickets-list']");

            await Page.FillAsync("[data-testid='search-input']", "login");
            await Page.PressAsync("[data-testid='search-input']", "Enter");

            await Page.WaitForSelectorAsync("[data-testid='ticket-item']");
            var searchResults = await Page.QuerySelectorAllAsync("[data-testid='ticket-item']");
            
            foreach (var result in searchResults)
            {
                var text = await result.TextContentAsync();
                text.ToLower().Should().Contain("login");
            }

            await Page.SelectOptionAsync("[data-testid='priority-filter']", "High");
            await Page.WaitForSelectorAsync("[data-testid='ticket-item']");

            var highPriorityTickets = await Page.QuerySelectorAllAsync("[data-testid='ticket-item']");
            foreach (var ticket in highPriorityTickets)
            {
                var priorityBadge = await ticket.QuerySelectorAsync("[data-testid='priority-badge']");
                var priorityText = await priorityBadge.TextContentAsync();
                priorityText.Should().Be("High");
            }
        }

        [Test]
        public async Task UserCanManageProjectMembers()
        {
            await LoginUser("project-manager@example.com", "TestPassword123!");
            await Page.ClickAsync("[data-testid='nav-projects']");
            await Page.WaitForSelectorAsync("[data-testid='projects-list']");

            await Page.ClickAsync("[data-testid='project-item']:first-child");
            await Page.WaitForSelectorAsync("[data-testid='project-details']");

            await Page.ClickAsync("[data-testid='project-members-tab']");
            await Page.WaitForSelectorAsync("[data-testid='members-list']");

            await Page.ClickAsync("[data-testid='add-member-btn']");
            await Page.FillAsync("[data-testid='member-email']", "newmember@example.com");
            await Page.SelectOptionAsync("[data-testid='member-role']", "Developer");
            await Page.ClickAsync("[data-testid='invite-member-btn']");

            await Page.WaitForSelectorAsync("text=newmember@example.com");
            var memberEmail = await Page.TextContentAsync("text=newmember@example.com");
            memberEmail.Should().Contain("newmember@example.com");

            await Page.ClickAsync("[data-testid='edit-member-role']");
            await Page.SelectOptionAsync("[data-testid='member-role']", "ProjectManager");
            await Page.ClickAsync("[data-testid='update-role-btn']");

            await Page.WaitForSelectorAsync("[data-testid='role-projectmanager']");
            var roleBadge = await Page.QuerySelectorAsync("[data-testid='role-projectmanager']");
            roleBadge.Should().NotBeNull();
        }

        [Test]
        public async Task UnauthorizedUserCannotAccessAdminFeatures()
        {
            await LoginUser("user@example.com", "TestPassword123!");

            await Page.GotoAsync($"{_baseUrl}/admin");
            
            await Page.WaitForSelectorAsync("[data-testid='access-denied']", new PageWaitForSelectorOptions { Timeout = 5000 });
            var accessDenied = await Page.QuerySelectorAsync("[data-testid='access-denied']");
            accessDenied.Should().NotBeNull();

            var response = await Page.APIRequest.GetAsync($"{_apiUrl}/api/tickets/admin/deleted");
            response.Status.Should().Be(401);
        }

        private async Task LoginUser(string email, string password)
        {
            await Page.ClickAsync("[data-testid='login-btn']");
            await Page.WaitForSelectorAsync("[data-testid='login-form']");
            
            await Page.FillAsync("[data-testid='email-input']", email);
            await Page.FillAsync("[data-testid='password-input']", password);
            await Page.ClickAsync("[data-testid='submit-login']");
            
            await Page.WaitForSelectorAsync("[data-testid='user-dashboard']", new PageWaitForSelectorOptions { Timeout = 10000 });
            
            var userMenu = await Page.QuerySelectorAsync("[data-testid='user-menu']");
            userMenu.Should().NotBeNull();
        }
    }
}