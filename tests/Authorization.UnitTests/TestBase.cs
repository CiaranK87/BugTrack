namespace Authorization.UnitTests;

public class TestBase : IDisposable
{
    protected readonly DataContext _context;
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ProjectRoleHandler _projectRoleHandler;
    protected readonly TicketAuthorizationHandler _ticketAuthorizationHandler;
    protected readonly Mock<IUserAccessor> _mockUserAccessor;

    public TestBase()
    {
        var services = new ServiceCollection();
        
        services.AddDbContext<DataContext>(options =>
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

        services.AddLogging();

        _mockUserAccessor = new Mock<IUserAccessor>();
        services.AddSingleton(_mockUserAccessor.Object);

        services.AddAuthorization(options =>
        {
            options.AddPolicy("ProjectOwnerOrManager", policy =>
                policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager")));
            options.AddPolicy("ProjectDeveloper", policy =>
                policy.Requirements.Add(new ProjectRoleRequirement("Developer")));
            options.AddPolicy("ProjectUser", policy =>
                policy.Requirements.Add(new ProjectRoleRequirement("User")));
            options.AddPolicy("ProjectContributor", policy =>
                policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager", "Developer", "User")));
            options.AddPolicy("ProjectAnyRole", policy =>
                policy.Requirements.Add(new ProjectRoleRequirement("Owner", "ProjectManager", "Developer", "User")));
        });
        
        services.AddSingleton<IAuthorizationHandler, ProjectRoleHandler>();
        services.AddSingleton<IAuthorizationHandler, TicketAuthorizationHandler>();

        _serviceProvider = services.BuildServiceProvider();
        _context = _serviceProvider.GetRequiredService<DataContext>();
        
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        _projectRoleHandler = new ProjectRoleHandler(
            _context,
            _mockUserAccessor.Object,
            loggerFactory.CreateLogger<ProjectRoleHandler>());
        
        _ticketAuthorizationHandler = new TicketAuthorizationHandler(
            _context,
            _mockUserAccessor.Object);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var users = new List<AppUser>
        {
            new AppUser
            {
                Id = "user1",
                UserName = "owner@test.com",
                Email = "owner@test.com",
                DisplayName = "Test Owner",
                GlobalRole = "Owner"
            },
            new AppUser
            {
                Id = "user2", 
                UserName = "admin@test.com",
                Email = "admin@test.com",
                DisplayName = "Test Admin",
                GlobalRole = "Admin"
            },
            new AppUser
            {
                Id = "user3",
                UserName = "projectmanager@test.com", 
                Email = "projectmanager@test.com",
                DisplayName = "Test Project Manager",
                GlobalRole = "ProjectManager"
            },
            new AppUser
            {
                Id = "user4",
                UserName = "developer@test.com",
                Email = "developer@test.com", 
                DisplayName = "Test Developer",
                GlobalRole = "User"
            },
            new AppUser
            {
                Id = "user5",
                UserName = "user@test.com",
                Email = "user@test.com",
                DisplayName = "Test User",
                GlobalRole = "User"
            }
        };

        var projects = new List<Project>
        {
            new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project 1",
                Description = "Test project description",
                StartDate = DateTime.UtcNow,
                IsCancelled = false,
                IsDeleted = false
            },
            new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project 2", 
                Description = "Another test project",
                StartDate = DateTime.UtcNow,
                IsCancelled = false,
                IsDeleted = false
            }
        };

        var participants = new List<ProjectParticipant>
        {
            new ProjectParticipant
            {
                AppUserId = "user1",
                ProjectId = projects[0].Id,
                Role = "Owner",
                IsOwner = true
            },
            new ProjectParticipant
            {
                AppUserId = "user3", 
                ProjectId = projects[0].Id,
                Role = "ProjectManager",
                IsOwner = false
            },
            new ProjectParticipant
            {
                AppUserId = "user4",
                ProjectId = projects[0].Id,
                Role = "Developer",
                IsOwner = false
            },
            new ProjectParticipant
            {
                AppUserId = "user5",
                ProjectId = projects[0].Id,
                Role = "User", 
                IsOwner = false
            },
            new ProjectParticipant
            {
                AppUserId = "user2",
                ProjectId = projects[1].Id,
                Role = "Owner",
                IsOwner = true
            }
        };

        var tickets = new List<Ticket>
        {
            new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket 1",
                Description = "Test ticket description",
                ProjectId = projects[0].Id,
                Submitter = "user4",
                Assigned = "user5",
                Status = "Open",
                Priority = "Medium",
                Severity = "Minor",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Updated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket 2",
                Description = "Another test ticket",
                ProjectId = projects[0].Id,
                Submitter = "user5",
                Assigned = "user4",
                Status = "In Progress",
                Priority = "High",
                Severity = "Major", 
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1),
                Updated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        _context.Users.AddRange(users);
        _context.Projects.AddRange(projects);
        _context.ProjectParticipants.AddRange(participants);
        _context.Tickets.AddRange(tickets);
        _context.SaveChanges();
    }

    protected AuthorizationHandlerContext CreateAuthorizationContext(string userId, object resource, params IAuthorizationRequirement[] requirements)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        var userName = user?.UserName ?? $"user{userId}";
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, $"{userName}@test.com"),
            new Claim("globalrole", GetUserGlobalRole(userId))
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        
        _mockUserAccessor.Setup(x => x.GetUserId()).Returns(userId);
        
        return new AuthorizationHandlerContext(requirements.ToList(), principal, resource);
    }

    protected AuthorizationHandlerContext CreateAuthorizationContextWithRouteData(string userId, object resource, Dictionary<string, object> routeData, params IAuthorizationRequirement[] requirements)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        var userName = user?.UserName ?? $"user{userId}";
        
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Email, $"{userName}@test.com"),
            new Claim("globalrole", GetUserGlobalRole(userId))
        };

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
        
        _mockUserAccessor.Setup(x => x.GetUserId()).Returns(userId);

        if (routeData != null && routeData.Any())
        {
            var httpContext = new DefaultHttpContext();
            foreach (var kvp in routeData)
            {
                httpContext.Request.RouteValues[kvp.Key] = kvp.Value.ToString();
            }
            
            if (resource == null)
            {
                return new AuthorizationHandlerContext(requirements.ToList(), principal, httpContext);
            }
        }

        return new AuthorizationHandlerContext(requirements.ToList(), principal, resource);
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
            _ => "User"
        };
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}