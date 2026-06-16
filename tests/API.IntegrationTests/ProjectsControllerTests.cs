using System.Net;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using Xunit;

namespace API.IntegrationTests
{
    [Collection("IntegrationTests")]
    public class ProjectsControllerTests
    {
        private (HttpClient client, DataContext context) CreateClient(Action<HttpClient> authenticate)
        {
            var factory = new TestProgram();
            var client = factory.CreateClient();
            var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            authenticate(client);
            return (client, context);
        }

        [Fact]
        public async Task ToggleCancel_AdminNonOwner_WhenActive_Returns403WithNoMutation()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsAdmin());

            var participantCountBefore = await context.ProjectParticipants
                .CountAsync(pp => pp.ProjectId == TestProgram.Project2Id);

            var response = await client.PostAsync($"/api/projects/{TestProgram.Project2Id}/cancel", null);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var project = await context.Projects.AsNoTracking()
                .FirstAsync(p => p.Id == TestProgram.Project2Id);
            project.IsCancelled.Should().BeFalse();

            var participantCountAfter = await context.ProjectParticipants
                .CountAsync(pp => pp.ProjectId == TestProgram.Project2Id);
            participantCountAfter.Should().Be(participantCountBefore);

            var adminIsParticipant = await context.ProjectParticipants
                .AnyAsync(pp => pp.ProjectId == TestProgram.Project2Id && pp.AppUserId == TestProgram.AdminUserId);
            adminIsParticipant.Should().BeFalse();
        }

        [Fact]
        public async Task ToggleCancel_Owner_WhenActive_Returns200AndCancels()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsDeveloper());

            var response = await client.PostAsync($"/api/projects/{TestProgram.Project2Id}/cancel", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var project = await context.Projects.AsNoTracking()
                .FirstAsync(p => p.Id == TestProgram.Project2Id);
            project.IsCancelled.Should().BeTrue();
        }

        [Fact]
        public async Task ToggleCancel_AdminNonOwner_WhenCancelled_Returns403WithNoMutation()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsAdmin());

            var project = await context.Projects.FindAsync(TestProgram.Project2Id);
            project!.IsCancelled = true;
            await context.SaveChangesAsync();

            var participantCountBefore = await context.ProjectParticipants
                .CountAsync(pp => pp.ProjectId == TestProgram.Project2Id);

            var response = await client.PostAsync($"/api/projects/{TestProgram.Project2Id}/cancel", null);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

            var projectAfter = await context.Projects.AsNoTracking()
                .FirstAsync(p => p.Id == TestProgram.Project2Id);
            projectAfter.IsCancelled.Should().BeTrue();

            var participantCountAfter = await context.ProjectParticipants
                .CountAsync(pp => pp.ProjectId == TestProgram.Project2Id);
            participantCountAfter.Should().Be(participantCountBefore);

            var adminIsParticipant = await context.ProjectParticipants
                .AnyAsync(pp => pp.ProjectId == TestProgram.Project2Id && pp.AppUserId == TestProgram.AdminUserId);
            adminIsParticipant.Should().BeFalse();
        }

        [Fact]
        public async Task ToggleCancel_Owner_WhenCancelled_Returns200AndReopens()
        {
            var (client, context) = CreateClient(c => c.AuthenticateAsDeveloper());

            var project = await context.Projects.FindAsync(TestProgram.Project2Id);
            project!.IsCancelled = true;
            await context.SaveChangesAsync();

            var response = await client.PostAsync($"/api/projects/{TestProgram.Project2Id}/cancel", null);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var projectAfter = await context.Projects.AsNoTracking()
                .FirstAsync(p => p.Id == TestProgram.Project2Id);
            projectAfter.IsCancelled.Should().BeFalse();
        }
    }
}
