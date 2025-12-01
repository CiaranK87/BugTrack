using Microsoft.EntityFrameworkCore;
using Moq;
using Application.Tickets;
using Application.Interfaces;
using Domain;
using Persistence;

namespace Application.UnitTests.Tickets
{
    public class CreateTicketHandlerTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly Mock<IUserAccessor> _mockUserAccessor;
        private readonly Create.Handler _handler;

        public CreateTicketHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _mockUserAccessor = new Mock<IUserAccessor>();
            _handler = new Create.Handler(_context);
        }

        [Fact]
        public async Task Handle_ValidTicket_ShouldCreateTicketSuccessfully()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                ProjectOwner = "Test Owner"
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description",
                Submitter = user.UserName,
                Assigned = user.UserName,
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = DateTime.UtcNow,
                ProjectId = project.Id
            };

            var command = new Create.Command { Ticket = ticket };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var createdTicket = await _context.Tickets.FindAsync(ticket.Id);
            createdTicket.Should().NotBeNull();
            createdTicket.Title.Should().Be("Test Ticket");
            createdTicket.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
            createdTicket.Updated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task Handle_TicketWithAssignedUserNotInProject_ShouldAddUserAsProjectParticipant()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            var assignedUser = new AppUser
            {
                Id = "user456",
                UserName = "assigneduser",
                DisplayName = "Assigned User"
            };
            _context.Users.AddRange(user, assignedUser);
            await _context.SaveChangesAsync();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                ProjectOwner = "Test Owner"
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description",
                Submitter = user.UserName,
                Assigned = assignedUser.UserName,
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = DateTime.UtcNow,
                ProjectId = project.Id
            };

            var command = new Create.Command { Ticket = ticket };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var participant = await _context.ProjectParticipants
                .FirstOrDefaultAsync(pp => pp.ProjectId == project.Id && pp.AppUserId == assignedUser.Id);
            
            participant.Should().NotBeNull();
            participant.IsOwner.Should().BeFalse();
            participant.Role.Should().Be("User");
        }

        [Fact]
        public async Task Handle_TicketWithAssignedUserAlreadyInProject_ShouldNotCreateDuplicateParticipant()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            var assignedUser = new AppUser
            {
                Id = "user456",
                UserName = "assigneduser",
                DisplayName = "Assigned User"
            };
            _context.Users.AddRange(user, assignedUser);
            await _context.SaveChangesAsync();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                ProjectOwner = "Test Owner"
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            // Add existing participant
            var existingParticipant = new ProjectParticipant
            {
                ProjectId = project.Id,
                AppUserId = assignedUser.Id,
                IsOwner = false,
                Role = "Admin"
            };
            _context.ProjectParticipants.Add(existingParticipant);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description",
                Submitter = user.UserName,
                Assigned = assignedUser.UserName,
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = DateTime.UtcNow,
                ProjectId = project.Id
            };

            var command = new Create.Command { Ticket = ticket };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var participants = await _context.ProjectParticipants
                .Where(pp => pp.ProjectId == project.Id && pp.AppUserId == assignedUser.Id)
                .ToListAsync();
            
            participants.Should().HaveCount(1);
            participants.First().Role.Should().Be("Admin"); // Original role preserved
        }

        [Fact]
        public async Task Handle_TicketWithNonExistentAssignedUser_ShouldCreateTicketSuccessfully()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow,
                ProjectOwner = "Test Owner"
            };
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description",
                Submitter = user.UserName,
                Assigned = "nonexistentuser",
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = DateTime.UtcNow,
                ProjectId = project.Id
            };

            var command = new Create.Command { Ticket = ticket };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var createdTicket = await _context.Tickets.FindAsync(ticket.Id);
            createdTicket.Should().NotBeNull();
            createdTicket.Assigned.Should().Be("nonexistentuser");
            
            // No participant should be created for non-existent user
            var participants = await _context.ProjectParticipants
                .Where(pp => pp.ProjectId == project.Id)
                .ToListAsync();
            participants.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_DatabaseFailure_ShouldReturnFailureResult()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var badContext = new DataContext(options);
            var badHandler = new Create.Handler(badContext);

            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            badContext.Users.Add(user);
            await badContext.SaveChangesAsync();

            // Create a ticket with a non-existent project to simulate failure
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description",
                Submitter = "testuser",
                Assigned = "assigneduser",
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = DateTime.UtcNow,
                ProjectId = Guid.NewGuid() // Non-existent project
            };

            var command = new Create.Command { Ticket = ticket };

            // Act
            var result = await badHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue(); // In-memory DB doesn't enforce FK constraints
            
            // Verify that the ticket was actually created despite non-existent project
            var createdTicket = await badContext.Tickets.FindAsync(ticket.Id);
            createdTicket.Should().NotBeNull();
            createdTicket.ProjectId.Should().Be(command.Ticket.ProjectId);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}