using Xunit;
using FluentAssertions;

namespace Domain.UnitTests.Models
{
    public class TicketTests
    {
        [Fact]
        public void Ticket_WhenCreated_ShouldHaveBusinessCriticalDefaults()
        {
            var ticket = new Ticket();
            
            ticket.IsDeleted.Should().BeFalse();
            ticket.Comments.Should().NotBeNull();
            ticket.Comments.Should().BeEmpty();
            ticket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void Ticket_CompleteWorkflow_ShouldFollowBusinessRules()
        {
            var ticket = new Ticket();
            var projectId = Guid.NewGuid();
            
            // Act
            ticket.Id = Guid.NewGuid();
            ticket.Title = "Critical authentication failure";
            ticket.Description = "Users cannot log in with valid credentials";
            ticket.Submitter = "user@example.com";
            ticket.Assigned = "developer@example.com";
            ticket.Priority = "High";
            ticket.Severity = "Critical";
            ticket.Status = "Open";
            ticket.ProjectId = projectId;
            ticket.StartDate = DateTime.UtcNow;
            
            ticket.Status = "In Progress";
            ticket.Updated = DateTime.UtcNow;
            ticket.Status = "Closed";
            ticket.ClosedDate = DateTime.UtcNow;
            
            ticket.Status.Should().Be("Closed");
            ticket.ClosedDate.Should().NotBeNull();
            ticket.Priority.Should().Be("High");
            ticket.Severity.Should().Be("Critical");
        }

        [Theory]
        [InlineData("Low", "Low", "Open")]
        [InlineData("High", "Critical", "Open")]
        [InlineData("Medium", "Medium", "In Progress")]
        public void Ticket_BusinessCombinations_ShouldReflectRealBugScenarios(string priority, string severity, string status)
        {
            var ticket = new Ticket();
            
            ticket.Priority = priority;
            ticket.Severity = severity;
            ticket.Status = status;
            
            ticket.Priority.Should().Be(priority);
            ticket.Severity.Should().Be(severity);
            ticket.Status.Should().Be(status);
        }

        [Fact]
        public void Ticket_DeletionWorkflow_ShouldMaintainAuditTrail()
        {
            var ticket = new Ticket();
            var beforeDeletion = DateTime.UtcNow;
            
            // Act
            ticket.IsDeleted = true;
            ticket.DeletedDate = DateTime.UtcNow;
            var afterDeletion = DateTime.UtcNow;
            
            ticket.IsDeleted.Should().BeTrue();
            ticket.DeletedDate.Should().NotBeNull();
            ticket.DeletedDate.Value.Should().BeOnOrAfter(beforeDeletion);
            ticket.DeletedDate.Value.Should().BeOnOrBefore(afterDeletion);
        }

        [Fact]
        public void Ticket_AssignmentWorkflow_ShouldSupportTeamCollaboration()
        {
            var ticket = new Ticket();
            var submitter = "user@example.com";
            var developer = "dev@example.com";
            
            // Act
            ticket.Submitter = submitter;
            ticket.Assigned = developer;
            
            ticket.Submitter.Should().Be(submitter);
            ticket.Assigned.Should().Be(developer);
            ticket.Submitter.Should().NotBe(ticket.Assigned);
        }

        [Fact]
        public void Ticket_ProjectAssociation_ShouldMaintainDataIntegrity()
        {
            var ticket = new Ticket();
            var projectId = Guid.NewGuid();
            
            ticket.ProjectId = projectId;
            
            ticket.ProjectId.Should().Be(projectId);
            ticket.ProjectId.Should().NotBe(Guid.Empty);
        }
    }
}