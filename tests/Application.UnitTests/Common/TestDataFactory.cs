using Bogus;
using Domain;
using Application.DTOs;
using Application.Projects;
using System.Security.Claims;

namespace Application.UnitTests.Common
{
    public static class TestDataFactory
    {
        private static readonly Faker<Ticket> TicketFaker = new Faker<Ticket>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.Title, f => f.Lorem.Sentence(3, 6))
            .RuleFor(t => t.Description, f => f.Lorem.Paragraph())
            .RuleFor(t => t.Priority, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(t => t.Severity, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(t => t.Status, f => f.PickRandom("Open", "In Progress", "Closed"))
            .RuleFor(t => t.Submitter, f => f.Internet.UserName())
            .RuleFor(t => t.Assigned, f => f.Internet.UserName())
            .RuleFor(t => t.StartDate, f => f.Date.Recent())
            .RuleFor(t => t.EndDate, f => f.Date.Future())
            .RuleFor(t => t.CreatedAt, f => f.Date.Recent())
            .RuleFor(t => t.Updated, f => f.Date.Recent())
            .RuleFor(t => t.ProjectId, f => f.Random.Guid())
            .RuleFor(t => t.IsDeleted, false);

        private static readonly Faker<Project> ProjectFaker = new Faker<Project>()
            .RuleFor(p => p.Id, f => f.Random.Guid())
            .RuleFor(p => p.ProjectTitle, f => f.Company.CompanyName() + " Project")
            .RuleFor(p => p.ProjectOwner, f => f.Name.FullName())
            .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
            .RuleFor(p => p.StartDate, f => f.Date.Recent())
            .RuleFor(p => p.IsCancelled, false)
            .RuleFor(p => p.IsDeleted, false);

        private static readonly Faker<AppUser> UserFaker = new Faker<AppUser>()
            .RuleFor(u => u.Id, f => f.Random.Guid().ToString())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.DisplayName, f => f.Name.FullName())
            .RuleFor(u => u.JobTitle, f => f.Name.JobTitle())
            .RuleFor(u => u.Bio, f => f.Lorem.Paragraph())
            .RuleFor(u => u.GlobalRole, f => f.PickRandom("User", "Developer", "ProjectManager", "Admin"))
            .RuleFor(u => u.IsDeleted, false)
            .RuleFor(u => u.JoinDate, f => f.Date.Past());

        public static Ticket CreateTicket(Guid? projectId = null)
        {
            var ticket = TicketFaker.Generate();
            if (projectId.HasValue)
                ticket.ProjectId = projectId.Value;
            return ticket;
        }

        public static List<Ticket> CreateTickets(int count, Guid? projectId = null)
        {
            var tickets = new List<Ticket>();
            for (int i = 0; i < count; i++)
            {
                tickets.Add(CreateTicket(projectId));
            }
            return tickets;
        }

        public static Project CreateProject()
        {
            return ProjectFaker.Generate();
        }

        public static List<Project> CreateProjects(int count)
        {
            return ProjectFaker.Generate(count);
        }

        public static AppUser CreateUser()
        {
            return UserFaker.Generate();
        }

        public static List<AppUser> CreateUsers(int count)
        {
            return UserFaker.Generate(count);
        }

        public static TicketDto CreateTicketDto(Guid? projectId = null)
        {
            var ticket = CreateTicket(projectId);
            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Priority = ticket.Priority,
                Severity = ticket.Severity,
                Status = ticket.Status,
                Submitter = ticket.Submitter,
                Assigned = ticket.Assigned,
                StartDate = ticket.StartDate,
                EndDate = ticket.EndDate,
                ProjectId = ticket.ProjectId,
                CreatedAt = ticket.CreatedAt,
                Updated = ticket.Updated
            };
        }

        public static ProjectDto CreateProjectDto()
        {
            var project = CreateProject();
            return new ProjectDto
            {
                Id = project.Id,
                ProjectTitle = project.ProjectTitle,
                ProjectOwner = project.ProjectOwner,
                Description = project.Description,
                StartDate = project.StartDate,
                IsCancelled = project.IsCancelled,
                IsDeleted = project.IsDeleted
            };
        }

        public static ClaimsPrincipal CreateClaimsPrincipal(string userId, string userName, string globalRole = "User")
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userName),
                new Claim("globalrole", globalRole)
            };

            return new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        }
    }
}