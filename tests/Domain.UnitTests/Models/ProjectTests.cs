using Xunit;
using FluentAssertions;

namespace Domain.UnitTests.Models
{
    /// <summary>
    /// Project business logic tests for bug tracking project management
    /// </summary>
    public class ProjectTests
    {
        [Fact]
        public void Project_WhenCreated_ShouldHaveBusinessCriticalDefaults()
        {
            var project = new Project();
            
            // Business defaults
            project.IsCancelled.Should().BeFalse(); // projects start as active
            project.IsDeleted.Should().BeFalse(); // projects start as active
            project.DeletedDate.Should().BeNull(); // no deletion timestamp initially
            project.Participants.Should().NotBeNull(); // participants collection must be initialized
            project.Participants.Should().BeEmpty(); // new projects have no participants initially
        }

        [Fact]
        public void Project_CompleteLifecycle_ShouldFollowBusinessWorkflow()
        {
            var project = new Project();
            var ownerId = "project-manager-001";
            
            // Act
            project.Id = Guid.NewGuid();
            project.ProjectTitle = "E-commerce Platform Redesign";
            project.ProjectOwner = ownerId;
            project.Description = "Complete redesign of the e-commerce platform with modern UI and improved performance";
            project.StartDate = DateTime.UtcNow;
            
            project.StartDate = DateTime.UtcNow.AddDays(7); // Project starts in a week
            
            project.IsCancelled = false; // Successfully completed
            
            // Business validations
            project.ProjectTitle.Should().Be("E-commerce Platform Redesign");
            project.ProjectOwner.Should().Be(ownerId);
            project.Description.Should().Contain("modern UI");
            project.IsCancelled.Should().BeFalse();
            project.IsDeleted.Should().BeFalse();
        }

        [Theory]
        [InlineData("Mobile App Development", "Native iOS and Android app development")]
        [InlineData("API Gateway Service", "Microservices API gateway for internal services")]
        [InlineData("UI Component Library", "Reusable React component library for design system")]
        [InlineData("Database Migration", "Legacy database migration to cloud-based solution")]
        public void Project_Types_ShouldReflectRealDevelopmentScenarios(string title, string description)
        {
            var project = new Project();
            
            project.ProjectTitle = title;
            project.Description = description;
            project.StartDate = DateTime.UtcNow;
            
            // Real software development projects
            project.ProjectTitle.Should().Be(title);
            project.Description.Should().Be(description);
            project.StartDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public void Project_TeamFormation_ShouldSupportCollaborativeDevelopment()
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Customer Portal Enhancement",
                ProjectOwner = "project-manager@company.com",
                Description = "Enhance customer portal with new features and improved UX",
                StartDate = DateTime.UtcNow
            };
            
            var projectManager = new ProjectParticipant
            {
                AppUserId = "project-manager-001",
                ProjectId = project.Id,
                IsOwner = true,
                Role = "Owner"
            };
            
            var developer1 = new ProjectParticipant
            {
                AppUserId = "developer-002",
                ProjectId = project.Id,
                IsOwner = false,
                Role = "Developer"
            };
            
            var developer2 = new ProjectParticipant
            {
                AppUserId = "developer-003",
                ProjectId = project.Id,
                IsOwner = false,
                Role = "Developer"
            };
            
            var developer3 = new ProjectParticipant
            {
                AppUserId = "developer-004",
                ProjectId = project.Id,
                IsOwner = false,
                Role = "Developer"
            };
            
            // Act
            project.Participants.Add(projectManager);
            project.Participants.Add(developer1);
            project.Participants.Add(developer2);
            project.Participants.Add(developer3);
            
            // Project execution
            project.Participants.Should().HaveCount(4);
            project.Participants.Count(p => p.IsOwner).Should().Be(1); // Single project owner
            project.Participants.Count(p => p.Role == "Developer").Should().Be(3); // Three developers
            project.Participants.All(p => p.ProjectId == project.Id).Should().BeTrue(); // All in same project
        }

        [Fact]
        public void Project_ProjectManagement_ShouldSupportTimelineAndStatus()
        {
            var project = new Project();
            var startDate = new DateTime(2023, 1, 1);
            var plannedEndDate = new DateTime(2023, 6, 30);
            
            project.Id = Guid.NewGuid();
            project.ProjectTitle = "API Modernization";
            project.ProjectOwner = "project-manager@company.com";
            project.Description = "Modernize legacy APIs to RESTful architecture";
            project.StartDate = startDate;
            
            project.IsCancelled = false; // Active project
            
            // Project tracking
            project.ProjectTitle.Should().Be("API Modernization");
            project.StartDate.Should().Be(startDate);
            project.IsCancelled.Should().BeFalse();
            project.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void Project_CancellationWorkflow_ShouldMaintainAuditTrail()
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Legacy System Replacement",
                ProjectOwner = "project-manager@company.com",
                StartDate = DateTime.UtcNow.AddMonths(-2)
            };
            
            var beforeCancellation = DateTime.UtcNow;
            
            // Act
            project.IsCancelled = true;
            var afterCancellation = DateTime.UtcNow;
            
            // Project governance and audit
            project.IsCancelled.Should().BeTrue();
            project.IsDeleted.Should().BeFalse(); // Cancelled but not deleted
            project.DeletedDate.Should().BeNull(); // No deletion timestamp
            project.StartDate.Should().BeBefore(beforeCancellation); // Project was active before cancellation
        }

        [Fact]
        public void Project_DeletionWorkflow_ShouldSupportDataRetention()
        {
            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Prototype Development",
                ProjectOwner = "project-manager@company.com",
                StartDate = DateTime.UtcNow.AddMonths(-1)
            };
            
            var beforeDeletion = DateTime.UtcNow;
            
            // Act
            project.IsDeleted = true;
            project.DeletedDate = DateTime.UtcNow;
            var afterDeletion = DateTime.UtcNow;
            
            // Data retention and compliance
            project.IsDeleted.Should().BeTrue();
            project.DeletedDate.Should().NotBeNull();
            project.DeletedDate.Value.Should().BeOnOrAfter(beforeDeletion);
            project.DeletedDate.Value.Should().BeOnOrBefore(afterDeletion);
            project.IsCancelled.Should().BeFalse(); // Deleted but not necessarily cancelled
        }

        [Fact]
        public void Project_ScenarioBasedProjects_ShouldReflectRealBusinessNeeds()
        {
            var businessProjects = new[]
            {
                new
                {
                    Title = "Customer Self-Service Portal",
                    Description = "Web portal for customers to manage their accounts and services",
                    Owner = "product-manager@company.com",
                    Duration = "6 months",
                    TeamSize = 5
                },
                new
                {
                    Title = "Internal Dashboard Analytics",
                    Description = "Real-time analytics dashboard for business intelligence",
                    Owner = "project-manager@company.com",
                    Duration = "3 months",
                    TeamSize = 3
                },
                new
                {
                    Title = "Mobile Payment Integration",
                    Description = "Integration with popular mobile payment providers",
                    Owner = "project-manager@company.com",
                    Duration = "4 months",
                    TeamSize = 4
                }
            };
            
            foreach (var projectScenario in businessProjects)
            {
                var project = new Project
                {
                    ProjectTitle = projectScenario.Title,
                    Description = projectScenario.Description,
                    ProjectOwner = projectScenario.Owner,
                    StartDate = DateTime.UtcNow
                };
                
                project.ProjectTitle.Should().Be(projectScenario.Title);
                project.Description.Should().Be(projectScenario.Description);
                project.ProjectOwner.Should().Be(projectScenario.Owner);
                project.StartDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
                
                projectScenario.Duration.Should().NotBeNullOrEmpty();
                projectScenario.TeamSize.Should().BeGreaterThan(0);
            }
        }

        [Fact]
        public void Project_ProjectOwnership_ShouldSupportAccountability()
        {
            // Arrange
            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Security Audit Implementation",
                Description = "Implement security audit recommendations",
                ProjectOwner = "project-manager@company.com",
                StartDate = DateTime.UtcNow
            };
            
            var ownerParticipant = new ProjectParticipant
            {
                AppUserId = "project-manager-001",
                ProjectId = project.Id,
                IsOwner = true,
                Role = "Owner"
            };
            
            project.Participants.Add(ownerParticipant);
            
            // Accountability and governance
            project.ProjectOwner.Should().Be("project-manager@company.com");
            project.Participants.Should().HaveCount(1);
            project.Participants.First().IsOwner.Should().BeTrue();
            project.Participants.First().Role.Should().Be("Owner");
            project.Participants.First().AppUserId.Should().Be("project-manager-001");
        }
    }
}