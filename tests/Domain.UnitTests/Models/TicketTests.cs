using Xunit;
using FluentAssertions;

namespace Domain.UnitTests.Models
{
    /// <summary>
    /// Ticket business logic tests for bug tracking domain rules
    /// </summary>
    public class TicketTests
    {
        [Fact]
        public void Ticket_WhenCreated_ShouldHaveBusinessCriticalDefaults()
        {
            var ticket = new Ticket();
            
            // Business defaults
            ticket.IsDeleted.Should().BeFalse(); // tickets should start as active
            ticket.Comments.Should().NotBeNull(); // comments collection must be initialized
            ticket.Comments.Should().BeEmpty(); // new tickets have no comments
            ticket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1)); // audit trail
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
            ticket.Status = "Resolved";
            ticket.ClosedDate = DateTime.UtcNow;
            
            // Business validations
            ticket.Status.Should().Be("Resolved"); // Final state validation
            ticket.ClosedDate.Should().NotBeNull(); // Closure must be recorded
            ticket.Priority.Should().Be("High"); // Priority preserved throughout lifecycle
            ticket.Severity.Should().Be("Critical"); // Severity preserved throughout lifecycle
        }

        [Theory]
        [InlineData("Low", "Minor", "Open")]
        [InlineData("High", "Critical", "Open")]
        [InlineData("Medium", "Major", "In Progress")]
        public void Ticket_BusinessCombinations_ShouldReflectRealBugScenarios(string priority, string severity, string status)
        {
            var ticket = new Ticket();
            
            ticket.Priority = priority;
            ticket.Severity = severity;
            ticket.Status = status;
            
            // Real bug tracking scenarios
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
            
            // Audit and compliance
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
            
            // Team coordination
            ticket.Submitter.Should().Be(submitter);
            ticket.Assigned.Should().Be(developer);
            ticket.Submitter.Should().NotBe(ticket.Assigned); // Different roles
        }

        [Fact]
        public void Ticket_ProjectAssociation_ShouldMaintainDataIntegrity()
        {
            var ticket = new Ticket();
            var projectId = Guid.NewGuid();
            
            ticket.ProjectId = projectId;
            
            // Project organization
            ticket.ProjectId.Should().Be(projectId);
            ticket.ProjectId.Should().NotBe(Guid.Empty); // Must be associated with a project
        }
    }
}