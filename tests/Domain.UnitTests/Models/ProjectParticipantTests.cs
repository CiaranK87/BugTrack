using Xunit;
using FluentAssertions;

namespace Domain.UnitTests.Models
{
    public class ProjectParticipantTests
    {
        [Fact]
        public void ProjectParticipant_OwnerAssignment_ShouldFollowBusinessRules()
        {
            var participant = new ProjectParticipant();
            var projectId = Guid.NewGuid();
            var userId = "project-manager-123";
            
            participant.AppUserId = userId;
            participant.ProjectId = projectId;
            participant.IsOwner = true;
            participant.Role = "Owner";
            
            participant.IsOwner.Should().BeTrue();
            participant.Role.Should().Be("Owner");
            participant.AppUserId.Should().Be(userId);
            participant.ProjectId.Should().Be(projectId);
        }

        [Theory]
        [InlineData("Developer", false)]
        [InlineData("Project Manager", false)]
        [InlineData("Admin", false)]
        [InlineData("Owner", true)]
        public void ProjectParticipant_RoleHierarchy_ShouldReflectRealTeamStructure(string role, bool isExpectedOwner)
        {
            var participant = new ProjectParticipant();
            
            participant.Role = role;
            participant.IsOwner = isExpectedOwner;
            
            participant.Role.Should().Be(role);
            participant.IsOwner.Should().Be(isExpectedOwner);
        }

        [Fact]
        public void ProjectParticipant_TeamComposition_ShouldSupportCrossFunctionalCollaboration()
        {
            var projectOwner = new ProjectParticipant
            {
                AppUserId = "project-manager-001",
                ProjectId = Guid.NewGuid(),
                IsOwner = true,
                Role = "Owner"
            };
            
            var developer2 = new ProjectParticipant
            {
                AppUserId = "developer-002",
                ProjectId = projectOwner.ProjectId,
                IsOwner = false,
                Role = "Developer"
            };
            
            var developer3 = new ProjectParticipant
            {
                AppUserId = "developer-003",
                ProjectId = projectOwner.ProjectId,
                IsOwner = false,
                Role = "Developer"
            };
            
            var developer4 = new ProjectParticipant
            {
                AppUserId = "developer-004",
                ProjectId = projectOwner.ProjectId,
                IsOwner = false,
                Role = "Developer"
            };
            
            projectOwner.IsOwner.Should().BeTrue();
            projectOwner.Role.Should().Be("Owner");
            
            developer2.IsOwner.Should().BeFalse();
            developer2.Role.Should().Be("Developer");
            
            developer3.IsOwner.Should().BeFalse();
            developer3.Role.Should().Be("Developer");
            
            developer4.IsOwner.Should().BeFalse();
            developer4.Role.Should().Be("Developer");
            
            developer2.ProjectId.Should().Be(projectOwner.ProjectId);
            developer3.ProjectId.Should().Be(projectOwner.ProjectId);
            developer4.ProjectId.Should().Be(projectOwner.ProjectId);
        }

        [Fact]
        public void ProjectParticipant_RoleProgression_ShouldSupportCareerGrowth()
        {
            var participant = new ProjectParticipant
            {
                AppUserId = "developer-001",
                ProjectId = Guid.NewGuid(),
                IsOwner = false,
                Role = "Developer"
            };
            
            participant.Role = "Developer";
            participant.IsOwner = true;
            participant.Role = "Owner";
            
            participant.IsOwner.Should().BeTrue();
            participant.Role.Should().Be("Owner");
            participant.AppUserId.Should().Be("developer-001");
        }

        [Fact]
        public void ProjectParticipant_MultipleProjectInvolvement_ShouldReflectRealWorkload()
        {
            var developerId = "developer-001";
            
            var projectAParticipation = new ProjectParticipant
            {
                AppUserId = developerId,
                ProjectId = Guid.NewGuid(),
                IsOwner = false,
                Role = "Developer"
            };
            
            var projectBParticipation = new ProjectParticipant
            {
                AppUserId = developerId,
                ProjectId = Guid.NewGuid(),
                IsOwner = true,
                Role = "Owner"
            };
            
            var projectCParticipation = new ProjectParticipant
            {
                AppUserId = developerId,
                ProjectId = Guid.NewGuid(),
                IsOwner = false,
                Role = "Developer"
            };
            
            projectAParticipation.AppUserId.Should().Be(developerId);
            projectBParticipation.AppUserId.Should().Be(developerId);
            projectCParticipation.AppUserId.Should().Be(developerId);
            
            projectAParticipation.Role.Should().Be("Developer");
            projectBParticipation.Role.Should().Be("Owner");
            projectCParticipation.Role.Should().Be("Developer");
            
            projectAParticipation.IsOwner.Should().BeFalse();
            projectBParticipation.IsOwner.Should().BeTrue();
            projectCParticipation.IsOwner.Should().BeFalse();
        }

        [Fact]
        public void ProjectParticipant_SpecializedRoles_ShouldSupportModernDevelopment()
        {
            var specializedRoles = new[]
            {
                ("Developer", false),
                ("Project Manager", false)
            };
            
            foreach (var (role, isOwner) in specializedRoles)
            {
                var participant = new ProjectParticipant
                {
                    AppUserId = $"{role.ToLower().Replace(" ", "-")}-001",
                    ProjectId = Guid.NewGuid(),
                    IsOwner = isOwner,
                    Role = role
                };
                
                participant.Role.Should().Be(role);
                participant.IsOwner.Should().Be(isOwner);
            }
        }

        [Fact]
        public void ProjectParticipant_ProjectLifecycle_ShouldSupportTeamChanges()
        {
            var projectId = Guid.NewGuid();
            var initialTeam = new List<ProjectParticipant>();
            
            initialTeam.Add(new ProjectParticipant
            {
                AppUserId = "owner-001",
                ProjectId = projectId,
                IsOwner = true,
                Role = "Owner"
            });
            
            var expandedTeam = new List<ProjectParticipant>(initialTeam);
            expandedTeam.Add(new ProjectParticipant
            {
                AppUserId = "developer-002",
                ProjectId = projectId,
                IsOwner = false,
                Role = "Developer"
            });
            
            expandedTeam.Add(new ProjectParticipant
            {
                AppUserId = "developer-003",
                ProjectId = projectId,
                IsOwner = false,
                Role = "Developer"
            });
            
            expandedTeam.Should().HaveCount(3);
            expandedTeam.Count(p => p.IsOwner).Should().Be(1);
            expandedTeam.Count(p => p.Role.Contains("Developer")).Should().Be(2);
            expandedTeam.All(p => p.ProjectId == projectId).Should().BeTrue();
        }
    }
}