namespace Application.UnitTests.Tickets
{
    public class CreateTicketTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly Create.Handler _handler;
        private readonly ServiceProvider _serviceProvider;

        public CreateTicketTests()
        {
            var services = new ServiceCollection();
            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));
            
            _serviceProvider = services.BuildServiceProvider();
            _context = _serviceProvider.GetRequiredService<DataContext>();
            _handler = new Create.Handler(_context);
        }

        [Fact]
        public async Task Handle_ValidTicket_ShouldReturnSuccessResult()
        {
            var ticket = TestDataFactory.CreateTicket();
            var command = new Create.Command { Ticket = ticket };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(Unit.Value);
            
            var savedTicket = await _context.Tickets.FindAsync(ticket.Id);
            savedTicket.Should().NotBeNull();
            savedTicket.Title.Should().Be(ticket.Title);
            savedTicket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public async Task Handle_TicketWithAssignedUser_ShouldAddParticipantIfNotExists()
        {
            var project = TestDataFactory.CreateProject();
            var assignedUser = TestDataFactory.CreateUser();
            var ticket = TestDataFactory.CreateTicket(project.Id);
            ticket.Assigned = assignedUser.UserName;

            _context.Projects.Add(project);
            _context.Users.Add(assignedUser);
            await _context.SaveChangesAsync();

            var command = new Create.Command { Ticket = ticket };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var savedTicket = await _context.Tickets.FindAsync(ticket.Id);
            savedTicket.Should().NotBeNull();
            
            var participant = await _context.ProjectParticipants
                .FirstOrDefaultAsync(pp => pp.ProjectId == project.Id && pp.AppUserId == assignedUser.Id);
            participant.Should().NotBeNull();
            participant.IsOwner.Should().BeFalse();
            participant.Role.Should().Be("User");
        }

        [Fact]
        public async Task Handle_TicketWithExistingParticipant_ShouldNotAddDuplicateParticipant()
        {
            var project = TestDataFactory.CreateProject();
            var assignedUser = TestDataFactory.CreateUser();
            var ticket = TestDataFactory.CreateTicket(project.Id);
            ticket.Assigned = assignedUser.UserName;

            var existingParticipant = new ProjectParticipant
            {
                ProjectId = project.Id,
                AppUserId = assignedUser.Id,
                IsOwner = true,
                Role = "Admin"
            };

            _context.Projects.Add(project);
            _context.Users.Add(assignedUser);
            _context.ProjectParticipants.Add(existingParticipant);
            await _context.SaveChangesAsync();

            var command = new Create.Command { Ticket = ticket };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var participants = await _context.ProjectParticipants
                .Where(pp => pp.ProjectId == project.Id && pp.AppUserId == assignedUser.Id)
                .ToListAsync();
            participants.Should().HaveCount(1);
            participants.First().IsOwner.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_TicketWithNonExistentAssignedUser_ShouldNotAddParticipant()
        {
            var project = TestDataFactory.CreateProject();
            var ticket = TestDataFactory.CreateTicket(project.Id);
            ticket.Assigned = "nonexistentuser";

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var command = new Create.Command { Ticket = ticket };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var participants = await _context.ProjectParticipants
                .Where(pp => pp.ProjectId == project.Id)
                .ToListAsync();
            participants.Should().BeEmpty();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
            _serviceProvider.Dispose();
        }
    }
}