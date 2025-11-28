namespace API.IntegrationTests
{
    public class TestProgram : WebApplicationFactory<Program>
    {
        public static Guid Project1Id { get; private set; } = Guid.Parse("87654321-4321-4321-4321-210987654321");
        public static Guid Project2Id { get; private set; } = Guid.Parse("98765432-1234-1234-1234-123456789012");
        public static Guid Ticket1Id { get; private set; } = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static Guid Ticket2Id { get; private set; } = Guid.Parse("22222222-2222-2222-2222-222222222222");
        public static string AdminUserId { get; private set; } = string.Empty;
        public static string TestUserId { get; private set; } = string.Empty;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove all existing DbContext registrations
                var dbContextDescriptors = services.Where(
                    d => d.ServiceType == typeof(DbContextOptions<DataContext>) ||
                           d.ServiceType == typeof(DataContext)).ToList();
                foreach (var descriptor in dbContextDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Remove all existing UserManager registrations
                var userManagerDescriptors = services.Where(
                    d => d.ServiceType == typeof(UserManager<AppUser>)).ToList();
                foreach (var descriptor in userManagerDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Remove all existing RoleManager registrations
                var roleManagerDescriptors = services.Where(
                    d => d.ServiceType == typeof(RoleManager<IdentityRole>)).ToList();
                foreach (var descriptor in roleManagerDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Add unique in-memory database for each test run
                var dbName = $"TestDb_{Guid.NewGuid()}";
                services.AddDbContext<DataContext>(options =>
                {
                    options.UseInMemoryDatabase(dbName);
                });

                // Add test user and role managers
                services.AddIdentityCore<AppUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<DataContext>();

                // Configure test authentication
                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "Test";
                    options.DefaultChallengeScheme = "Test";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

                // Register IHttpContextAccessor and IUserAccessor
                services.AddHttpContextAccessor();
                services.AddScoped<IUserAccessor, UserAccessor>();

                // Add services for testing
                services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Application.Tickets.List.Handler).Assembly));
                services.AddAutoMapper(typeof(Application.Core.MappingProfiles).Assembly);

                // Create the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to get the services
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<DataContext>();
                var logger = scopedServices.GetRequiredService<ILogger<TestProgram>>();

                try
                {
                    // Ensure the database is created
                    db.Database.EnsureCreated();

                    // Seed test data
                    SeedTestData(db, scopedServices);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the database.");
                }
            });
        }

        private void SeedTestData(DataContext db, IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<AppUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Create roles
            roleManager.CreateAsync(new IdentityRole("Admin")).Wait();
            roleManager.CreateAsync(new IdentityRole("ProjectManager")).Wait();
            roleManager.CreateAsync(new IdentityRole("Developer")).Wait();
            roleManager.CreateAsync(new IdentityRole("User")).Wait();

            // Create admin user
            var adminUser = new AppUser
            {
                UserName = "admin",
                Email = "admin@test.com",
                DisplayName = "Admin User"
            };
            userManager.CreateAsync(adminUser, "Password123!").Wait();
            userManager.AddToRoleAsync(adminUser, "Admin").Wait();
            AdminUserId = adminUser.Id;

            // Create test user
            var testUser = new AppUser
            {
                UserName = "testuser",
                Email = "test@example.com",
                DisplayName = "Test User"
            };
            userManager.CreateAsync(testUser, "Password123!").Wait();
            userManager.AddToRoleAsync(testUser, "Developer").Wait();
            TestUserId = testUser.Id;

            // Create test projects
            var project1 = new Project
            {
                Id = Guid.Parse("87654321-4321-4321-4321-210987654321"),
                ProjectTitle = "Test Project 1",
                Description = "Test Description 1",
                StartDate = DateTime.UtcNow,
                ProjectOwner = adminUser.UserName,
                IsCancelled = false
            };
            Project1Id = project1.Id;
            db.Projects.Add(project1);

            var project2 = new Project
            {
                Id = Guid.Parse("98765432-1234-1234-1234-123456789012"),
                ProjectTitle = "Test Project 2",
                Description = "Test Description 2",
                StartDate = DateTime.UtcNow,
                ProjectOwner = testUser.UserName,
                IsCancelled = false
            };
            Project2Id = project2.Id;
            db.Projects.Add(project2);

            // Create test tickets with fixed IDs for predictable tests
            var testTicket1 = new Ticket
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Test Ticket 1",
                Description = "Test Description",
                ProjectId = project1.Id,
                Submitter = adminUser.UserName,
                Assigned = testUser.UserName,
                Priority = "Medium",
                Severity = "Minor",
                Status = "Open",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                Updated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            Ticket1Id = testTicket1.Id;
            db.Tickets.Add(testTicket1);

            var testTicket2 = new Ticket
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Test Ticket 2",
                Description = "Another test ticket",
                ProjectId = project2.Id,
                Submitter = testUser.UserName,
                Assigned = testUser.UserName,
                Priority = "High",
                Severity = "Major",
                Status = "In Progress",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(7),
                Updated = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
            Ticket2Id = testTicket2.Id;
            db.Tickets.Add(testTicket2);

            // Create project participants with proper roles
            var participant1 = new ProjectParticipant
            {
                AppUserId = adminUser.Id,
                ProjectId = project1.Id,
                Role = "Owner",
                IsOwner = true
            };
            db.ProjectParticipants.Add(participant1);

            var participant2 = new ProjectParticipant
            {
                AppUserId = testUser.Id,
                ProjectId = project1.Id,
                Role = "Developer",
                IsOwner = false
            };
            db.ProjectParticipants.Add(participant2);

            var participant3 = new ProjectParticipant
            {
                AppUserId = testUser.Id,
                ProjectId = project2.Id,
                Role = "Owner",
                IsOwner = true
            };
            db.ProjectParticipants.Add(participant3);

            // Save changes to ensure all entities are persisted
            db.SaveChanges();
            
            // Explicitly load the data to ensure relationships are properly established
            var projects = db.Projects
                .Include(p => p.Participants)
                .Include(p => p.Tickets)
                .ToList();
            
            var participants = db.ProjectParticipants.ToList();
            var tickets = db.Tickets.ToList();
            
            // Log the counts for debugging
            var logger = services.GetRequiredService<ILogger<TestProgram>>();
            logger.LogInformation($"Seeded {projects.Count} projects, {participants.Count} participants, {tickets.Count} tickets");
        }
    }

    public static class HttpClientExtensions
    {
        public static void AuthenticateAs(this HttpClient client, string userId, string userName, string userEmail, string globalRole)
        {
            // Set the authorization header for our test authentication with JSON data
            var authData = new Dictionary<string, string>
            {
                ["userId"] = userId,
                ["userName"] = userName,
                ["userEmail"] = userEmail,
                ["globalrole"] = globalRole
            };
            
            var authJson = JsonSerializer.Serialize(authData);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test", authJson);
        }

        public static void AuthenticateAsAdmin(this HttpClient client)
        {
            client.AuthenticateAs(
                TestProgram.AdminUserId,
                "admin",
                "admin@test.com",
                "Admin"
            );
        }

        public static void AuthenticateAsDeveloper(this HttpClient client)
        {
            client.AuthenticateAs(
                TestProgram.TestUserId,
                "testuser",
                "test@example.com",
                "Developer"
            );
        }

        public static void AuthenticateAsDeveloperWithProjectRole(this HttpClient client, Guid projectId, string projectRole)
        {
            var authData = new Dictionary<string, string>
            {
                ["userId"] = TestProgram.TestUserId,
                ["userName"] = "testuser",
                ["userEmail"] = "test@example.com",
                ["globalrole"] = "Developer",
                [$"project_{projectId}"] = projectRole
            };
            
            var authJson = JsonSerializer.Serialize(authData);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Test", authJson);
        }
    }
}