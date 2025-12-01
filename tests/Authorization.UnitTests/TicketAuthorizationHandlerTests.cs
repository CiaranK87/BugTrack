using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Authorization;
using Application.Interfaces;
using Domain;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Persistence;
using Xunit;

namespace Authorization.UnitTests;

public class TicketAuthorizationHandlerTests : TestBase
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
            "user1" => "User",
            "user2" => "Admin",
            "user3" => "User",
            "user4" => "User",
            "user5" => "User",
            _ => "User"
        };
    }

    private async Task<Domain.Ticket> CreateTestTicketAsync()
    {
        var project = _context.Projects.First();
        var ticket = new Domain.Ticket
        {
            Id = Guid.NewGuid(),
            Title = "Test Ticket",
            Description = "Test Description",
            ProjectId = project.Id,
            Submitter = "user4",
            Assigned = "user5",
            Status = "Open",
            Priority = "Medium",
            Severity = "Low",
            CreatedAt = DateTime.UtcNow,
            Updated = DateTime.UtcNow
        };
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    [Theory]
    [InlineData("user1", TicketOperation.Read, true)]
    [InlineData("user1", TicketOperation.Create, true)]
    [InlineData("user1", TicketOperation.Edit, true)]
    [InlineData("user1", TicketOperation.Close, true)]
    [InlineData("user1", TicketOperation.Delete, false)]
    [InlineData("user3", TicketOperation.Read, true)]
    [InlineData("user3", TicketOperation.Create, true)]
    [InlineData("user3", TicketOperation.Edit, true)]
    [InlineData("user3", TicketOperation.Close, true)]
    [InlineData("user3", TicketOperation.Delete, false)]
    [InlineData("user4", TicketOperation.Read, true)]
    [InlineData("user4", TicketOperation.Create, true)]
    [InlineData("user4", TicketOperation.Edit, true)]
    [InlineData("user4", TicketOperation.Close, true)]
    [InlineData("user4", TicketOperation.Delete, false)]
    [InlineData("user5", TicketOperation.Read, true)]
    [InlineData("user5", TicketOperation.Create, true)]
    [InlineData("user5", TicketOperation.Close, false)]
    [InlineData("user5", TicketOperation.Delete, false)]
    public async Task TicketOperationAuthorizationTests(string userId, TicketOperation operation, bool shouldSucceed)
    {
        // Arrange
        var ticket = _context.Tickets.First();
        var requirement = new TicketOperationRequirement(operation);
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal(userId);

        // Act
        var result = await authService.AuthorizeAsync(user, ticket, requirement);

        // Assert
        if (shouldSucceed)
        {
            result.Succeeded.Should().BeTrue($"User {userId} should be able to {operation} ticket");
        }
        else
        {
            result.Succeeded.Should().BeFalse($"User {userId} should not be able to {operation} ticket");
        }
    }

    [Theory]
    [InlineData("user4", TicketOperation.Read, true)]
    [InlineData("user4", TicketOperation.Create, true)]
    [InlineData("user4", TicketOperation.Edit, true)]
    [InlineData("user4", TicketOperation.Close, true)]
    public async Task TicketSubmitter_ShouldHaveAppropriateAccess(string userId, TicketOperation operation, bool shouldSucceed)
    {
        // Arrange
        var ticket = _context.Tickets.First();
        var requirement = new TicketOperationRequirement(operation);
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal(userId);

        // Act
        var result = await authService.AuthorizeAsync(user, ticket, requirement);

        // Assert
        if (shouldSucceed)
        {
            result.Succeeded.Should().BeTrue($"Ticket submitter should be able to {operation} their own ticket");
        }
        else
        {
            result.Succeeded.Should().BeFalse($"Ticket submitter should not be able to {operation} their own ticket");
        }
    }

    [Theory]
    [InlineData("user5", TicketOperation.Read, true)]
    [InlineData("user5", TicketOperation.Create, true)]
    [InlineData("user5", TicketOperation.Close, false)]
    [InlineData("user5", TicketOperation.Delete, false)]
    public async Task TicketAssignedUser_ShouldHaveAppropriateAccess(string userId, TicketOperation operation, bool shouldSucceed)
    {
        // Arrange
        var ticket = _context.Tickets.First();
        var requirement = new TicketOperationRequirement(operation);
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal(userId);

        // Act
        var result = await authService.AuthorizeAsync(user, ticket, requirement);

        // Assert
        if (shouldSucceed)
        {
            result.Succeeded.Should().BeTrue($"Assigned user should be able to {operation} their assigned ticket");
        }
        else
        {
            result.Succeeded.Should().BeFalse($"Assigned user should not be able to {operation} their assigned ticket");
        }
    }

    [Theory]
    [InlineData("user4", TicketOperation.Read, true)]
    [InlineData("user4", TicketOperation.Create, true)]
    [InlineData("user4", TicketOperation.Edit, true)]
    [InlineData("user4", TicketOperation.Close, true)]
    public async Task UserIsBothSubmitterAndAssigned_ShouldHaveFullAccess(string userId, TicketOperation operation, bool shouldSucceed)
    {
        // Arrange
        var ticket = _context.Tickets.First();
        
        ticket.Assigned = "user4";
        await _context.SaveChangesAsync();
        
        var requirement = new TicketOperationRequirement(operation);
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal(userId);

        // Act
        var result = await authService.AuthorizeAsync(user, ticket, requirement);

        // Assert
        if (shouldSucceed)
        {
            result.Succeeded.Should().BeTrue($"User who is both submitter and assigned should be able to {operation}");
        }
        else
        {
            result.Succeeded.Should().BeFalse($"User who is both submitter and assigned should not be able to {operation}");
        }
    }

    [Fact]
    public async Task AdminUser_ShouldBypassTicketAuthorization()
    {
        // Arrange
        var ticket = _context.Tickets.First();
        var requirement = new TicketOperationRequirement(TicketOperation.Delete);
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user2");

        // Act
        var result = await authService.AuthorizeAsync(user, ticket, requirement);

        // Assert
        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task NonParticipant_ShouldFailTicketAuthorization()
    {
        // Arrange
        var ticket = _context.Tickets.First();
        var requirement = new TicketOperationRequirement(TicketOperation.Read);
        var authService = _serviceProvider.GetRequiredService<IAuthorizationService>();
        var user = CreateUserPrincipal("user999");

        // Act
        var result = await authService.AuthorizeAsync(user, ticket, requirement);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

}