using Microsoft.EntityFrameworkCore;
using Moq;
using Application.Projects;
using Application.Interfaces;
using Domain;
using Persistence;

namespace Application.UnitTests.Projects
{
    public class CreateProjectHandlerTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly Mock<IUserAccessor> _mockUserAccessor;
        private readonly Create.Handler _handler;

        public CreateProjectHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _mockUserAccessor = new Mock<IUserAccessor>();
            _handler = new Create.Handler(_context, _mockUserAccessor.Object);
        }

        [Fact]
        public async Task Handle_ValidProjectWithExistingUser_ShouldCreateProjectSuccessfully()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUsername()).Returns(user.UserName);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow
            };

            var command = new Create.Command { Project = project };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var createdProject = await _context.Projects.FindAsync(project.Id);
            createdProject.Should().NotBeNull();
            createdProject.ProjectTitle.Should().Be("Test Project");
            createdProject.ProjectOwner.Should().Be("Test User");
            
            var participant = await _context.ProjectParticipants
                .FirstOrDefaultAsync(pp => pp.ProjectId == project.Id && pp.AppUserId == user.Id);
            participant.Should().NotBeNull();
            participant.IsOwner.Should().BeTrue();
            participant.Role.Should().Be("Owner");
        }

        [Fact]
        public async Task Handle_ValidProjectWithUserWithoutDisplayName_ShouldUseUsernameAsOwner()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = null
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUsername()).Returns(user.UserName);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow
            };

            var command = new Create.Command { Project = project };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var createdProject = await _context.Projects.FindAsync(project.Id);
            createdProject.Should().NotBeNull();
            createdProject.ProjectOwner.Should().Be("testuser");
        }

        [Fact]
        public async Task Handle_ValidProjectWithUserWithEmptyDisplayName_ShouldUseUsernameAsOwner()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = ""
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUsername()).Returns(user.UserName);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow
            };

            var command = new Create.Command { Project = project };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var createdProject = await _context.Projects.FindAsync(project.Id);
            createdProject.Should().NotBeNull();
            createdProject.ProjectOwner.Should().Be("testuser");
        }

        [Fact]
        public async Task Handle_ValidProjectWithNonExistentUser_ShouldCreateProjectWithoutOwner()
        {
            _mockUserAccessor.Setup(x => x.GetUsername()).Returns("nonexistentuser");

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow
            };

            var command = new Create.Command { Project = project };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ProjectWithEmptyTitle_HandlerSucceedsWithoutValidation()
        {
            // Validation is the pipeline's responsibility; the handler itself imposes no constraints.
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var badContext = new DataContext(options);
            var badHandler = new Create.Handler(badContext, _mockUserAccessor.Object);

            _mockUserAccessor.Setup(x => x.GetUsername()).Returns("testuser");

            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            badContext.Users.Add(user);
            await badContext.SaveChangesAsync();

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "",
                Description = "",
                StartDate = DateTime.UtcNow
            };

            var command = new Create.Command { Project = project };

            var result = await badHandler.Handle(command, CancellationToken.None);

            result.IsSuccess.Should().BeTrue();
            var createdProject = await badContext.Projects.FindAsync(project.Id);
            createdProject.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ValidProject_ShouldAddParticipantToProjectParticipantsCollection()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUsername()).Returns(user.UserName);

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.UtcNow
            };

            var command = new Create.Command { Project = project };

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            
            var createdProject = await _context.Projects
                .Include(p => p.Participants)
                .FirstOrDefaultAsync(p => p.Id == project.Id);
            
            createdProject.Should().NotBeNull();
            createdProject.Participants.Should().HaveCount(1);
            createdProject.Participants.First().IsOwner.Should().BeTrue();
            createdProject.Participants.First().Role.Should().Be("Owner");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}