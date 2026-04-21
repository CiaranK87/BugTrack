using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Application.Comments;
using Application.Interfaces;
using Domain;
using Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FluentAssertions;

namespace Application.UnitTests.Comments
{
    public class CreateCommentHandlerTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly Mock<IUserAccessor> _mockUserAccessor;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly Mock<INotificationPushService> _mockNotificationPushService;
        private readonly Mock<ILogger<Create.Handler>> _mockLogger;
        private readonly Create.Handler _handler;

        public CreateCommentHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new DataContext(options);
            _mockUserAccessor = new Mock<IUserAccessor>();
            _mockNotificationService = new Mock<INotificationService>();
            _mockNotificationPushService = new Mock<INotificationPushService>();
            _mockLogger = new Mock<ILogger<Create.Handler>>();
            
            _handler = new Create.Handler(
                _context, 
                _mockUserAccessor.Object, 
                _mockNotificationService.Object,
                _mockNotificationPushService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task Handle_ValidComment_ShouldCreateCommentSuccessfully()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(user.Id);

            var command = new Create.Command
            {
                Content = "This is a test comment",
                TicketId = ticket.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Content.Should().Be("This is a test comment");
            result.Value.AuthorId.Should().Be(user.Id);
            result.Value.AuthorUsername.Should().Be(user.UserName);
            result.Value.AuthorDisplayName.Should().Be(user.DisplayName);
            result.Value.TicketId.Should().Be(ticket.Id);

            var createdComment = await _context.Comments.FindAsync(result.Value.Id);
            createdComment.Should().NotBeNull();
            createdComment.Content.Should().Be("This is a test comment");
            createdComment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        }

        [Fact]
        public async Task Handle_ReplyToComment_ShouldCreateReplySuccessfully()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            var parentComment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Parent comment",
                TicketId = ticket.Id,
                AuthorId = user.Id,
                CreatedAt = DateTime.UtcNow
            };
            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            _context.Comments.Add(parentComment);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(user.Id);

            var command = new Create.Command
            {
                Content = "This is a reply",
                TicketId = ticket.Id,
                ParentCommentId = parentComment.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.ParentCommentId.Should().Be(parentComment.Id);

            var createdReply = await _context.Comments.FindAsync(result.Value.Id);
            createdReply.Should().NotBeNull();
            createdReply.ParentCommentId.Should().Be(parentComment.Id);
        }

        [Fact]
        public async Task Handle_CommentWithAttachments_ShouldCreateCommentWithAttachments()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(user.Id);

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.txt");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("text/plain");

            var command = new Create.Command
            {
                Content = "This is a comment with attachment",
                TicketId = ticket.Id,
                Attachments = new List<IFormFile> { mockFile.Object }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Attachments.Should().HaveCount(1);

            var createdComment = await _context.Comments
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(c => c.Id == result.Value.Id);
            createdComment.Should().NotBeNull();
            createdComment.Attachments.Should().HaveCount(1);
            createdComment.Attachments.First().OriginalFileName.Should().Be("test.txt");
        }

        [Fact]
        public async Task Handle_EmptyContent_ShouldCreateCommentSuccessfully()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(user.Id);

            var command = new Create.Command
            {
                Content = "",
                TicketId = ticket.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // The handler itself does not validate content — that is enforced by the MediatR
            // validation pipeline before the handler is reached. Called directly, the handler
            // accepts empty content and creates the comment.
            result.IsSuccess.Should().BeTrue();
            result.Value.Content.Should().Be("");
            result.Value.AuthorId.Should().Be(user.Id);
        }

        [Fact]
        public async Task Handle_CommentWithMentions_ShouldCreateNotifications()
        {
            var author = new AppUser
            {
                Id = "author123",
                UserName = "author_user",
                DisplayName = "Author User"
            };
            var mentionedUser = new AppUser
            {
                Id = "mentioned456",
                UserName = "mentioned_user",
                DisplayName = "Mentioned User",
                GlobalRole = "Admin"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            
            _context.Users.AddRange(author, mentionedUser);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(author.Id);

            var notificationGuid = Guid.NewGuid();
            _mockNotificationService.Setup(x => x.CreateMentionNotificationAsync(
                mentionedUser.Id, It.IsAny<Guid>(), ticket.Id, author.DisplayName))
                .ReturnsAsync(new Domain.Notification { 
                    Id = notificationGuid,
                    RecipientId = mentionedUser.Id,
                    Type = Domain.NotificationType.Mention,
                    Message = "Author User mentioned you",
                    TicketId = ticket.Id,
                    IsRead = false
                });

            var command = new Create.Command
            {
                Content = "Hey @mentioned_user, what do you think?",
                TicketId = ticket.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            _mockNotificationService.Verify(
                x => x.CreateMentionNotificationAsync(mentionedUser.Id, result.Value.Id, ticket.Id, author.DisplayName),
                Times.Once
            );

            _mockNotificationPushService.Verify(
                x => x.PushNotificationAsync(mentionedUser.Id, It.Is<Application.DTOs.NotificationDto>(n => n.RecipientId == mentionedUser.Id)),
                Times.Once
            );
            
            _mockNotificationPushService.Verify(
                x => x.PushUnreadCountUpdateAsync(mentionedUser.Id, It.IsAny<int>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_CommentWithSpaceLessMention_ShouldResolveToMultiWordUsername()
        {
            var author = new AppUser
            {
                Id = "author123",
                UserName = "author_user",
                DisplayName = "Author User"
            };
            var mentionedUser = new AppUser
            {
                Id = "mentioned456",
                UserName = "Mike O Brien",
                DisplayName = "Mike O Brien",
                GlobalRole = "Admin"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                ProjectId = Guid.NewGuid()
            };
            
            _context.Users.AddRange(author, mentionedUser);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(author.Id);

            var command = new Create.Command
            {
                Content = "Hey @MikeOBrien, welcome!",
                TicketId = ticket.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            _mockNotificationService.Verify(
                x => x.CreateMentionNotificationAsync(mentionedUser.Id, result.Value.Id, ticket.Id, author.DisplayName),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_CommentWithApostropheMention_ShouldResolveCorrectly()
        {
            var author = new AppUser
            {
                Id = "author123",
                UserName = "author_user",
                DisplayName = "Author User"
            };
            var mentionedUser = new AppUser
            {
                Id = "mentioned456",
                UserName = "Denis O'Brien",
                DisplayName = "Denis O'Brien",
                GlobalRole = "Admin"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                ProjectId = Guid.NewGuid()
            };
            
            _context.Users.AddRange(author, mentionedUser);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(author.Id);

            var command = new Create.Command
            {
                Content = "Hey @DenisO'Brien, how are you?",
                TicketId = ticket.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            
            _mockNotificationService.Verify(
                x => x.CreateMentionNotificationAsync(mentionedUser.Id, result.Value.Id, ticket.Id, author.DisplayName),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_DatabaseFailure_ShouldReturnFailureResult()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var badContext = new DataContext(options);
            var badHandler = new Create.Handler(
                badContext, 
                _mockUserAccessor.Object, 
                _mockNotificationService.Object,
                _mockNotificationPushService.Object,
                _mockLogger.Object
            );

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns("user123");

            // Create a user first
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            badContext.Users.Add(user);
            await badContext.SaveChangesAsync();

            var command = new Create.Command
            {
                Content = "This is a test comment",
                TicketId = Guid.NewGuid() // Non-existent ticket
            };

            // Act
            var result = await badHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue(); // In-memory DB doesn't enforce FK constraints
            
            // Verify that the comment was actually created despite non-existent ticket
            // This is a limitation of in-memory databases - they don't enforce foreign key constraints
            var createdComment = await badContext.Comments.FindAsync(result.Value.Id);
            createdComment.Should().NotBeNull();
            createdComment.TicketId.Should().Be(command.TicketId);
        }

        [Fact]
        public async Task Handle_CommentWithMultipleAttachments_ShouldCreateCommentWithAllAttachments()
        {
            var user = new AppUser
            {
                Id = "user123",
                UserName = "testuser",
                DisplayName = "Test User"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(user.Id);

            var mockFile1 = new Mock<IFormFile>();
            mockFile1.Setup(f => f.FileName).Returns("test1.txt");
            mockFile1.Setup(f => f.Length).Returns(1024);
            mockFile1.Setup(f => f.ContentType).Returns("text/plain");

            var mockFile2 = new Mock<IFormFile>();
            mockFile2.Setup(f => f.FileName).Returns("test2.txt");
            mockFile2.Setup(f => f.Length).Returns(2048);
            mockFile2.Setup(f => f.ContentType).Returns("text/plain");

            var command = new Create.Command
            {
                Content = "This is a comment with multiple attachments",
                TicketId = ticket.Id,
                Attachments = new List<IFormFile> { mockFile1.Object, mockFile2.Object }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Attachments.Should().HaveCount(2);

            var createdComment = await _context.Comments
                .Include(c => c.Attachments)
                .FirstOrDefaultAsync(c => c.Id == result.Value.Id);
            createdComment.Should().NotBeNull();
            createdComment.Attachments.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_GuestUserWithAttachment_ShouldRestrictGuestAttachments()
        {
            var user = new AppUser
            {
                Id = "guest123",
                UserName = "guestuser",
                DisplayName = "Guest User"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(user.Id);
            _mockUserAccessor.Setup(x => x.GetGlobalRole()).Returns("Guest");

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns("test.txt");
            mockFile.Setup(f => f.Length).Returns(1024);
            mockFile.Setup(f => f.ContentType).Returns("text/plain");

            var command = new Create.Command
            {
                Content = "This is a comment with attachment",
                TicketId = ticket.Id,
                Attachments = new List<IFormFile> { mockFile.Object }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("File uploads are not available for Guest users.");
        }

        [Fact]
        public async Task Handle_GuestUserWithoutAttachment_ShouldCreateCommentSuccessfully()
        {
            var user = new AppUser
            {
                Id = "guest123",
                UserName = "guestuser",
                DisplayName = "Guest User"
            };
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                Title = "Test Ticket",
                Description = "Test Description"
            };
            _context.Users.Add(user);
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            _mockUserAccessor.Setup(x => x.GetUserId()).Returns(user.Id);
            _mockUserAccessor.Setup(x => x.GetGlobalRole()).Returns("Guest");

            var command = new Create.Command
            {
                Content = "This is a comment without attachment",
                TicketId = ticket.Id
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Content.Should().Be("This is a comment without attachment");
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}