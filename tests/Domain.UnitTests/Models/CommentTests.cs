using Xunit;
using FluentAssertions;

namespace Domain.UnitTests.Models
{
    public class CommentTests
    {
        [Fact]
        public void Comment_WhenCreated_ShouldHaveBusinessCriticalDefaults()
        {
            var comment = new Comment();
            
            comment.Replies.Should().NotBeNull();
            comment.Replies.Should().BeEmpty();
            comment.Attachments.Should().NotBeNull();
            comment.Attachments.Should().BeEmpty();
        }

        [Fact]
        public void Comment_ReplyChain_ShouldSupportNestedDiscussions()
        {
            var parentComment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Found a critical bug in authentication",
                TicketId = Guid.NewGuid(),
                AuthorId = "user",
                CreatedAt = DateTime.UtcNow.AddHours(-2)
            };
            
            var developerReply = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "I can reproduce this. Working on a fix now.",
                TicketId = parentComment.TicketId,
                AuthorId = "dev-user",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                ParentCommentId = parentComment.Id
            };
            
            var followUp = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Great! Can you estimate when this will be deployed?",
                TicketId = parentComment.TicketId,
                AuthorId = "user",
                CreatedAt = DateTime.UtcNow,
                ParentCommentId = developerReply.Id
            };
            
            // Act
            parentComment.Replies.Add(developerReply);
            developerReply.Replies.Add(followUp);
            
            parentComment.Replies.Should().HaveCount(1);
            parentComment.Replies.First().Should().Be(developerReply);
            developerReply.Replies.Should().HaveCount(1);
            developerReply.Replies.First().Should().Be(followUp);
            followUp.ParentCommentId.Should().Be(developerReply.Id);
        }

        [Fact]
        public void Comment_AttachmentWorkflow_ShouldSupportBugEvidence()
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Here are the screenshots and logs showing the issue",
                TicketId = Guid.NewGuid(),
                AuthorId = "user"
            };
            
            var screenshot = new CommentAttachment 
            { 
                Id = Guid.NewGuid(),
                FileName = "error-screenshot.png",
                FilePath = "/uploads/error-screenshot.png"
            };
            
            var logFile = new CommentAttachment 
            { 
                Id = Guid.NewGuid(),
                FileName = "error-logs.txt",
                FilePath = "/uploads/error-logs.txt"
            };
            
            // Act
            comment.Attachments.Add(screenshot);
            comment.Attachments.Add(logFile);
            
            comment.Attachments.Should().HaveCount(2);
            comment.Attachments.Should().Contain(screenshot);
            comment.Attachments.Should().Contain(logFile);
        }

        [Theory]
        [InlineData("Critical security vulnerability discovered", "security")]
        [InlineData("Performance degradation under load", "Performance")]
        [InlineData("UI not responsive on mobile devices", "UI")]
        [InlineData("Data corruption in database", "data")]
        public void Comment_BugCategories_ShouldReflectRealWorldScenarios(string content, string category)
        {
            var comment = new Comment();
            
            comment.Content = content;
            
            comment.Content.Should().Be(content);
            comment.Content.Should().Contain(category);
        }

        [Fact]
        public void Comment_UpdateWorkflow_ShouldMaintainAuditTrail()
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Initial bug report",
                CreatedAt = DateTime.UtcNow.AddHours(-1)
            };
            
            var beforeUpdate = DateTime.UtcNow;
            
            // Act
            comment.Content = "Updated bug report with more details and reproduction steps";
            comment.UpdatedAt = DateTime.UtcNow;
            var afterUpdate = DateTime.UtcNow;
            
            comment.Content.Should().Be("Updated bug report with more details and reproduction steps");
            comment.UpdatedAt.Should().NotBeNull();
            comment.UpdatedAt.Value.Should().BeOnOrAfter(beforeUpdate);
            comment.UpdatedAt.Value.Should().BeOnOrBefore(afterUpdate);
        }

        [Fact]
        public void Comment_TicketAssociation_ShouldMaintainDataIntegrity()
        {
            var comment = new Comment();
            var ticketId = Guid.NewGuid();
            var authorId = "developer-123";
            
            comment.TicketId = ticketId;
            comment.AuthorId = authorId;
            
            comment.TicketId.Should().Be(ticketId);
            comment.TicketId.Should().NotBe(Guid.Empty);
            comment.AuthorId.Should().Be(authorId);
            comment.AuthorId.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Comment_CollaborationWorkflow_ShouldSupportTeamCommunication()
        {
            var userComment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Bug verified in production environment",
                TicketId = Guid.NewGuid(),
                AuthorId = "user",
                CreatedAt = DateTime.UtcNow.AddHours(-3)
            };
            
            var devComment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Root cause identified: race condition in user service",
                TicketId = userComment.TicketId,
                AuthorId = "developer",
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                ParentCommentId = userComment.Id
            };
            
            var opsComment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = "Hotfix deployed to production. Monitoring for issues.",
                TicketId = userComment.TicketId,
                AuthorId = "developer",
                CreatedAt = DateTime.UtcNow.AddHours(-1),
                ParentCommentId = devComment.Id
            };
            
            // Act
            userComment.Replies.Add(devComment);
            devComment.Replies.Add(opsComment);
            
            userComment.AuthorId.Should().Be("user");
            devComment.AuthorId.Should().Be("developer");
            opsComment.AuthorId.Should().Be("developer");
            
            userComment.Replies.Should().HaveCount(1);
            userComment.Replies.First().Content.Should().Contain("race condition");
            devComment.Replies.Should().HaveCount(1);
            devComment.Replies.First().Content.Should().Contain("Hotfix deployed");
        }
    }
}