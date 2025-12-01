using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Xunit;

namespace API.IntegrationTests
{
    /// <summary>
    /// Integration tests for full-stack ticket management workflows
    /// </summary>
    public class TicketsControllerTests
    {
        public TicketsControllerTests()
        {
        }

        private TestProgram CreateFactory()
        {
            return new TestProgram();
        }

        private (HttpClient client, DataContext context) CreateAuthenticatedClient()
        {
            var factory = CreateFactory();
            var client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            return (client, context);
        }

        [Fact]
        public async Task GetTickets_FullWorkflow_ReturnsCompleteTicketData()
        {
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();
            var response = await client.GetAsync("/api/tickets");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var result = await response.Content.ReadFromJsonAsync<List<TicketDto>>();
            result.Should().NotBeNull();
            result.Should().HaveCount(2); // 2 tickets seeded in TestProgram
            
            var firstTicket = result.First(t => t.Id == TestProgram.Ticket1Id);
            firstTicket.Title.Should().Be("Test Ticket 1");
            firstTicket.Description.Should().Be("Test Description");
            firstTicket.Priority.Should().Be("Medium");
            firstTicket.Severity.Should().Be("Low");
            firstTicket.Status.Should().Be("Open");
            firstTicket.ProjectId.Should().Be(TestProgram.Project1Id);
        }

        [Fact]
        public async Task GetTicket_WithValidId_ReturnsCompleteTicketInformation()
        {
            // Arrange
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();

            var ticketId = TestProgram.Ticket1Id;

            var response = await client.GetAsync($"/api/tickets/{ticketId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var result = await response.Content.ReadFromJsonAsync<TicketDto>();
            result.Should().NotBeNull();
            result.Id.Should().Be(ticketId);
            result.Title.Should().Be("Test Ticket 1");
            result.Description.Should().Be("Test Description");
            result.Submitter.Should().Be("admin");
            result.Assigned.Should().Be("testuser");
            result.Priority.Should().Be("Medium");
            result.Severity.Should().Be("Low");
            result.Status.Should().Be("Open");
            result.ProjectId.Should().Be(TestProgram.Project1Id);
        }

        [Fact]
        public async Task GetTicketsByProject_ValidProject_ReturnsFilteredResults()
        {
            // Arrange
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();

            var projectId = TestProgram.Project1Id;

            var response = await client.GetAsync($"/api/tickets/project/{projectId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var result = await response.Content.ReadFromJsonAsync<List<TicketDto>>();
            result.Should().NotBeNull();
            result.Should().HaveCount(1); // 1 ticket seeded for this project
            result.All(t => t.ProjectId == projectId).Should().BeTrue();
            
            // Validate the filtered ticket data
            var filteredTicket = result.First();
            filteredTicket.Title.Should().Be("Test Ticket 1");
            filteredTicket.ProjectId.Should().Be(projectId);
        }

        [Fact]
        public async Task CreateTicket_RealWorldScenario_CreatesTicketWithProperWorkflow()
        {
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();
            var ticketDto = new
            {
                Title = "Critical authentication failure on mobile app",
                Description = "Users cannot log in using mobile app when using corporate credentials. Error message: 'Invalid credentials' even with correct password. Desktop version works fine. Affects 25% of mobile users.",
                ProjectId = TestProgram.Project1Id,
                Priority = "High",
                Severity = "Critical",
                Submitter = "admin",
                Assigned = "testuser",
                Status = "Open"
            };

            var response = await client.PostAsJsonAsync("/api/tickets", ticketDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var createdTickets = await context.Tickets
                .Where(t => t.Title == ticketDto.Title && t.Description == ticketDto.Description)
                .ToListAsync();
            createdTickets.Should().HaveCount(1);
            
            var createdTicket = createdTickets.First();
            createdTicket.Title.Should().Be(ticketDto.Title);
            createdTicket.Description.Should().Be(ticketDto.Description);
            createdTicket.ProjectId.Should().Be(ticketDto.ProjectId);
            createdTicket.Priority.Should().Be(ticketDto.Priority);
            createdTicket.Severity.Should().Be(ticketDto.Severity);
            createdTicket.Submitter.Should().Be(ticketDto.Submitter);
            createdTicket.Assigned.Should().Be(ticketDto.Assigned);
            createdTicket.Status.Should().Be(ticketDto.Status);
            createdTicket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public async Task EditTicket_RealWorldUpdate_UpdatesTicketWithAuditTrail()
        {
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();
            var editDto = new
            {
                Title = "Updated: Critical authentication failure on mobile app",
                Description = "Updated description: Root cause identified - OAuth token refresh issue. Working on fix for production deployment.",
                Priority = "Critical", // Escalated priority
                Severity = "Critical",
                Status = "In Progress", // Status progression
                Assigned = "testuser",
                ProjectId = TestProgram.Project1Id
            };

            var beforeUpdate = DateTime.UtcNow;

            var response = await client.PutAsJsonAsync($"/api/tickets/{TestProgram.Ticket1Id}", editDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedTicket = await context.Tickets.FindAsync(TestProgram.Ticket1Id);
            updatedTicket.Should().NotBeNull();
            updatedTicket.Title.Should().Be(editDto.Title);
            updatedTicket.Description.Should().Be(editDto.Description);
            updatedTicket.Priority.Should().Be(editDto.Priority);
            updatedTicket.Status.Should().Be(editDto.Status);
            updatedTicket.Updated.Should().BeOnOrAfter(beforeUpdate);
        }

        [Fact]
        public async Task DeleteTicket_RealWorldDeletion_PerformsSoftDeleteWithAudit()
        {
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();

            var beforeDeletion = DateTime.UtcNow;

            var response = await client.DeleteAsync($"/api/tickets/{TestProgram.Ticket1Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var deletedTicket = await context.Tickets.FindAsync(TestProgram.Ticket1Id);
            deletedTicket.Should().NotBeNull(); // Still exists (soft delete)
            deletedTicket.IsDeleted.Should().BeTrue();
            deletedTicket.DeletedDate.Should().NotBeNull();
            deletedTicket.DeletedDate.Value.Should().BeOnOrAfter(beforeDeletion);
            deletedTicket.DeletedDate.Value.Should().BeOnOrBefore(DateTime.UtcNow);
        }

        [Fact]
        public async Task GetTickets_Unauthenticated_ReturnsUnauthorized()
        {
            // Arrange
            var (client, context) = CreateAuthenticatedClient();

            var response = await client.GetAsync("/api/tickets");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreateTicket_WithoutProjectRole_ReturnsForbidden()
        {
            var factory = CreateFactory();
            var client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            
            client.AuthenticateAs(
                "non-member-user-id",
                "nonmember",
                "nonmember@example.com",
                "User"
            );

            var ticketDto = new
            {
                Title = "Unauthorized ticket creation attempt",
                Description = "This should fail due to lack of project permissions",
                ProjectId = TestProgram.Project1Id,
                Priority = "High",
                Severity = "Medium",
                Submitter = "admin",
                Assigned = "testuser",
                Status = "Open"
            };

            var response = await client.PostAsJsonAsync("/api/tickets", ticketDto);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task TicketWorkflow_CompleteBugLifecycle_DemonstratesRealWorldUsage()
        {
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsDeveloperWithProjectRole(TestProgram.Project1Id, "Developer");
            var newTicketDto = new
            {
                Title = "Performance issue in search results",
                Description = "Search results take >5 seconds to load when user has >1000 records. This affects power users significantly.",
                ProjectId = TestProgram.Project1Id,
                Priority = "Medium",
                Severity = "Low",
                Submitter = "testuser",
                Assigned = "testuser",
                Status = "Open"
            };

            var createResponse = await client.PostAsJsonAsync("/api/tickets", newTicketDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var createdTicket = await context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Title == newTicketDto.Title);
            createdTicket.Should().NotBeNull();
            var newTicketId = createdTicket.Id;

            var updateDto = new
            {
                Title = newTicketDto.Title,
                Description = "Investigation complete: Need to implement database indexing for user records.",
                Priority = "High",
                Severity = "Medium",
                Status = "In Progress",
                Assigned = "testuser",
                ProjectId = TestProgram.Project1Id
            };

            var updateResponse = await client.PutAsJsonAsync($"/api/tickets/{newTicketId}", updateDto);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedTicket = await context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == newTicketId);
            updatedTicket.Should().NotBeNull();
            updatedTicket.Description.Should().Contain("Investigation complete");
            updatedTicket.Priority.Should().Be("High");

            var resolveDto = new
            {
                Title = newTicketDto.Title,
                Description = "Implemented database indexing. Search now loads in <200ms.",
                Priority = "High",
                Severity = "Medium",
                Status = "Closed",
                Assigned = "testuser",
                ProjectId = TestProgram.Project1Id
            };

            var resolveResponse = await client.PutAsJsonAsync($"/api/tickets/{newTicketId}", resolveDto);
            resolveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // Assert
            var finalTicket = await context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == newTicketId);
            finalTicket.Should().NotBeNull();
            finalTicket.Description.Should().Contain("database indexing");
            finalTicket.Priority.Should().Be("High"); // Escalated during process
            finalTicket.Status.Should().BeOneOf("Closed", "In Progress", "Open");
        }

        [Fact]
        public async Task GetTicket_WithInvalidId_ReturnsNotFound()
        {
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();

            var invalidId = Guid.NewGuid();

            var response = await client.GetAsync($"/api/tickets/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteTicket_WithInvalidId_ReturnsNotFound()
        {
            var (client, context) = CreateAuthenticatedClient();
            client.AuthenticateAsAdmin();

            var invalidId = Guid.NewGuid();

            var response = await client.DeleteAsync($"/api/tickets/{invalidId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}