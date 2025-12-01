using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Authorization;
using Application.Interfaces;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Persistence;
using Xunit;

namespace Authorization.UnitTests
{
    public class SimpleHandlerTests
    {
        private readonly DataContext _context;
        private readonly Mock<IUserAccessor> _userAccessorMock;
        private readonly Mock<ILogger<ProjectRoleHandler>> _loggerMock;
        private readonly ProjectRoleHandler _handler;

        public SimpleHandlerTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new DataContext(options);
            _userAccessorMock = new Mock<IUserAccessor>();
            _loggerMock = new Mock<ILogger<ProjectRoleHandler>>();
            _handler = new ProjectRoleHandler(_context, _userAccessorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task OwnerAccess_ShouldSucceed_WhenUserIsOwner()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "user1";
            
            // Create test data
            var user = new AppUser { Id = userId, UserName = "testuser" };
            var project = new Project { Id = projectId, ProjectTitle = "Test Project" };
            var participant = new ProjectParticipant 
            { 
                AppUserId = userId, 
                ProjectId = projectId, 
                Role = "Owner",
                IsOwner = true
            };

            _context.Users.Add(user);
            _context.Projects.Add(project);
            _context.ProjectParticipants.Add(participant);
            await _context.SaveChangesAsync();

            // Setup user accessor
            _userAccessorMock.Setup(x => x.GetUserId()).Returns(userId);

            // Create authorization context
            var claims = new[] { new Claim("globalrole", "User") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            var requirement = new ProjectRoleRequirement("Owner");
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                principal,
                projectId);

            // Act - Call the handler directly through the authorization framework
            await _handler.HandleAsync(context);

            // Assert
            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }


        [Fact]
        public async Task AdminAccess_ShouldSucceed_RegardlessOfProjectRole()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var userId = "user1";
            
            // Create test data - user is a Developer
            var user = new AppUser { Id = userId, UserName = "testuser" };
            var project = new Project { Id = projectId, ProjectTitle = "Test Project" };
            var participant = new ProjectParticipant 
            { 
                AppUserId = userId, 
                ProjectId = projectId, 
                Role = "Developer",
                IsOwner = false
            };

            _context.Users.Add(user);
            _context.Projects.Add(project);
            _context.ProjectParticipants.Add(participant);
            await _context.SaveChangesAsync();

            // Setup user accessor
            _userAccessorMock.Setup(x => x.GetUserId()).Returns(userId);

            // Create authorization context - user is Admin globally
            var claims = new[] { new Claim("globalrole", "Admin") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            
            var requirement = new ProjectRoleRequirement("Owner");
            var context = new AuthorizationHandlerContext(
                new[] { requirement },
                principal,
                projectId);

            // Act
            await _handler.HandleAsync(context);

            // Assert
            context.HasSucceeded.Should().BeTrue();
            context.HasFailed.Should().BeFalse();
        }
    }
}