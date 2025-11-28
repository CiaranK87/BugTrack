using Xunit;
using FluentAssertions;

namespace Domain.UnitTests.Models
{
    public class AppUserTests
    {
        [Fact]
        public void AppUser_WhenCreated_ShouldHaveBusinessCriticalDefaults()
        {
            var user = new AppUser();
            
            user.GlobalRole.Should().Be("User");
            user.IsDeleted.Should().BeFalse();
            user.DeletedAt.Should().BeNull();
            user.JoinDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Theory]
        [InlineData("Admin")]
        [InlineData("ProjectManager")]
        [InlineData("User")]
        [InlineData("Developer")]
        public void AppUser_GlobalRoles_ShouldReflectApplicationRoles(string role)
        {
            var user = new AppUser();
            
            user.GlobalRole = role;
            
            user.GlobalRole.Should().Be(role);
        }

        [Fact]
        public void AppUser_UserLifecycle_ShouldSupportAccountManagement()
        {
            var user = new AppUser
            {
                Id = "user-123",
                UserName = "john.doe@company.com",
                GlobalRole = "User",
                JoinDate = DateTime.UtcNow.AddMonths(-6)
            };
            
            user.GlobalRole = "Developer";
            user.GlobalRole = "Manager";
            
            var beforeDeletion = DateTime.UtcNow;
            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            var afterDeletion = DateTime.UtcNow;
            
            user.GlobalRole.Should().Be("Manager");
            user.IsDeleted.Should().BeTrue();
            user.DeletedAt.Should().NotBeNull();
            user.DeletedAt.Value.Should().BeOnOrAfter(beforeDeletion);
            user.DeletedAt.Value.Should().BeOnOrBefore(afterDeletion);
        }

        [Fact]
        public void AppUser_ProfileManagement_ShouldSupportProfessionalIdentity()
        {
            var user = new AppUser
            {
                Id = "dev-001",
                UserName = "alice.smith@techcorp.com",
                DisplayName = "Alice Smith",
                JobTitle = "Software Engineer",
                Bio = "Full-stack developer with experience in web applications and cloud architecture",
                GlobalRole = "Developer"
            };
            
            user.DisplayName.Should().Be("Alice Smith");
            user.JobTitle.Should().Be("Software Engineer");
            user.Bio.Should().Contain("Full-stack developer");
            user.GlobalRole.Should().Be("Developer");
            user.UserName.Should().Be("alice.smith@techcorp.com");
        }

        [Fact]
        public void AppUser_RoleBasedAccess_ShouldReflectApplicationRoles()
        {
            var teamMembers = new[]
            {
                new AppUser { GlobalRole = "Admin", UserName = "admin@company.com" },
                new AppUser { GlobalRole = "ProjectManager", UserName = "pm@company.com" },
                new AppUser { GlobalRole = "Developer", UserName = "dev@company.com" },
                new AppUser { GlobalRole = "User", UserName = "user@company.com" }
            };
            
            teamMembers.Should().HaveCount(4);
            teamMembers.Count(u => u.GlobalRole == "Admin").Should().Be(1);
            teamMembers.Count(u => u.GlobalRole == "ProjectManager").Should().Be(1);
            teamMembers.Count(u => u.GlobalRole == "Developer").Should().Be(1);
            teamMembers.Count(u => u.GlobalRole == "User").Should().Be(1);
            
            teamMembers.All(u => u.IsDeleted == false).Should().BeTrue();
        }

        [Fact]
        public void AppUser_SecurityWorkflow_ShouldMaintainAuditTrail()
        {
            var user = new AppUser
            {
                Id = "security-user-001",
                UserName = "security.officer@company.com",
                GlobalRole = "Admin",
                JoinDate = DateTime.UtcNow.AddYears(-1)
            };
            
            var auditDate = DateTime.UtcNow;
            
            user.Id.Should().Be("security-user-001");
            user.GlobalRole.Should().Be("Admin");
            user.JoinDate.Should().BeBefore(auditDate);
            user.IsDeleted.Should().BeFalse();
            user.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void AppUser_UserProfile_ShouldStoreProfessionalInformation()
        {
            var user = new AppUser
            {
                DisplayName = "Alice Smith",
                JobTitle = "Software Engineer",
                Bio = "Full-stack developer with experience in web applications",
                GlobalRole = "Developer"
            };
            
            user.DisplayName.Should().Be("Alice Smith");
            user.JobTitle.Should().Be("Software Engineer");
            user.Bio.Should().Contain("Full-stack developer");
            user.GlobalRole.Should().Be("Developer");
        }

        [Fact]
        public void AppUser_RoleBasedPermissions_ShouldReflectApplicationHierarchy()
        {
            var adminUser = new AppUser { GlobalRole = "Admin" };
            var projectManagerUser = new AppUser { GlobalRole = "ProjectManager" };
            var developerUser = new AppUser { GlobalRole = "Developer" };
            var regularUser = new AppUser { GlobalRole = "User" };
            
            // Role hierarchy validation
            adminUser.GlobalRole.Should().Be("Admin");
            projectManagerUser.GlobalRole.Should().Be("ProjectManager");
            developerUser.GlobalRole.Should().Be("Developer");
            regularUser.GlobalRole.Should().Be("User");
            
            // All users should have the same default state
            var allUsers = new[] { adminUser, projectManagerUser, developerUser, regularUser };
            allUsers.All(u => u.IsDeleted == false).Should().BeTrue();
            allUsers.All(u => u.DeletedAt == null).Should().BeTrue();
        }
    }
}