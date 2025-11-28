namespace Authorization.UnitTests;

public class ProjectRoleHandlerTests : TestBase
{
    private System.Security.Claims.ClaimsPrincipal CreateUserPrincipal(string userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        var userName = user?.UserName ?? $"user{userId}";
        
        var claims = new List<System.Security.Claims.Claim>
        {
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, userName),
            new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, $"{userName}@test.com"),
            new System.Security.Claims.Claim("globalrole", GetUserGlobalRole(userId))
        };

        var principal = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity(claims, "Test"));
        
        // Set up the mock to return the correct user ID for this test
        _mockUserAccessor.Setup(x => x.GetUserId()).Returns(userId);
        
        return principal;
    }

    private string GetUserGlobalRole(string userId)
    {
        return userId switch
        {
            "user1" => "Owner",
            "user2" => "Admin",
            "user3" => "ProjectManager",
            "user4" => "User",
            "user5" => "User",
            "user6" => "Admin",
            "user7" => "ProjectManager",
            "user8" => "User",
            "user9" => "User",
            _ => "User"
        };
    }


    [Fact]
    public async Task ProjectManagerParticipant_ShouldFailForDeveloperRequirement()
    {
        // Arrange
        var project = _context.Projects.First();
        var requirement = new ProjectRoleRequirement("Developer");
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user3");

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task DeveloperParticipant_ShouldFailForUserRequirement()
    {
        // Arrange
        var project = _context.Projects.First();
        var requirement = new ProjectRoleRequirement("User");
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user4");

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task UserParticipant_ShouldFailForOwnerRequirement()
    {
        // Arrange
        var project = _context.Projects.First();
        var requirement = new ProjectRoleRequirement("Owner");
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user5");

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task NonParticipant_ShouldFail()
    {
        // Arrange
        var project = _context.Projects.First();
        var requirement = new ProjectRoleRequirement("User");
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user999");

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Theory]
    [InlineData("user1", "Owner", "Developer", false)]
    [InlineData("user1", "Owner", "User", false)]
    [InlineData("user3", "ProjectManager", "Owner", true)]
    [InlineData("user3", "ProjectManager", "Developer", false)]
    [InlineData("user3", "ProjectManager", "User", false)]
    [InlineData("user4", "Developer", "Owner", false)]
    [InlineData("user4", "Developer", "ProjectManager", false)]
    [InlineData("user4", "Developer", "User", false)]
    [InlineData("user5", "User", "Owner", false)]
    [InlineData("user5", "User", "ProjectManager", false)]
    [InlineData("user5", "User", "Developer", false)]
    public async Task RoleHierarchyTests(string userId, string userRole, string requiredRole, bool shouldSucceed)
    {
        // Arrange
        var project = _context.Projects.First();
        var requirement = new ProjectRoleRequirement(requiredRole);
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal(userId);

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        if (shouldSucceed)
        {
            result.Succeeded.Should().BeTrue($"User {userRole} should be able to access {requiredRole}");
        }
        else
        {
            result.Succeeded.Should().BeFalse($"User {userRole} should not be able to access {requiredRole}");
        }
    }

    [Fact]
    public async Task AdminUser_ShouldBypassRoleCheck()
    {
        // Arrange
        var project = _context.Projects.First();
        var requirement = new ProjectRoleRequirement("Owner");
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user2");

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task ProjectManagerUser_ShouldAccessOwnerRequirement()
    {
        // Arrange
        var project = _context.Projects.First();
        var requirement = new ProjectRoleRequirement("Owner");
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user3");

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task ContributorRole_ShouldNotBeTreatedAsUser()
    {
        // Arrange
        var project = _context.Projects.First();
        var contributorParticipant = new ProjectParticipant
        {
            AppUserId = "user9",
            ProjectId = project.Id,
            Role = "Contributor",
            IsOwner = false
        };
        _context.ProjectParticipants.Add(contributorParticipant);
        await _context.SaveChangesAsync();

        var requirement = new ProjectRoleRequirement("User");
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user9");

        // Act
        var result = await authService.AuthorizeAsync(user, project, requirement);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

}