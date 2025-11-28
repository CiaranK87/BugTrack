namespace API.IntegrationTests.Common
{
    public static class TestDataFactory
    {
        private static readonly Faker<Ticket> TicketFaker = new Faker<Ticket>()
            .RuleFor(t => t.Id, f => Guid.NewGuid())
            .RuleFor(t => t.Title, f => f.Lorem.Sentence())
            .RuleFor(t => t.Description, f => f.Lorem.Paragraphs())
            .RuleFor(t => t.Priority, f => f.PickRandom(new[] { "Low", "Medium", "High", "Critical" }))
            .RuleFor(t => t.Severity, f => f.PickRandom(new[] { "Minor", "Major", "Critical" }))
            .RuleFor(t => t.Status, f => f.PickRandom(new[] { "Open", "In Progress", "Resolved", "Closed" }))
            .RuleFor(t => t.Submitter, f => f.Internet.UserName())
            .RuleFor(t => t.Assigned, f => f.Internet.UserName())
            .RuleFor(t => t.CreatedAt, f => f.Date.Recent())
            .RuleFor(t => t.Updated, f => f.Date.Recent())
            .RuleFor(t => t.StartDate, f => f.Date.Recent())
            .RuleFor(t => t.EndDate, f => f.Date.Future());

        private static readonly Faker<Project> ProjectFaker = new Faker<Project>()
            .RuleFor(p => p.Id, f => Guid.NewGuid())
            .RuleFor(p => p.ProjectTitle, f => f.Company.CompanyName())
            .RuleFor(p => p.Description, f => f.Lorem.Sentence())
            .RuleFor(p => p.ProjectOwner, f => f.Name.FullName())
            .RuleFor(p => p.StartDate, f => f.Date.Recent());

        private static readonly Faker<AppUser> UserFaker = new Faker<AppUser>()
            .RuleFor(u => u.Id, f => Guid.NewGuid().ToString())
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.DisplayName, f => f.Name.FullName());

        public static Ticket CreateTicket(Guid? projectId = null)
        {
            var ticket = TicketFaker.Generate();
            if (projectId.HasValue)
                ticket.ProjectId = projectId.Value;
            return ticket;
        }

        public static Project CreateProject()
        {
            return ProjectFaker.Generate();
        }

        public static AppUser CreateUser()
        {
            return UserFaker.Generate();
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
                CreatedAt = ticket.CreatedAt,
                Updated = ticket.Updated,
                ProjectId = ticket.ProjectId,
                Submitter = ticket.Submitter,
                Assigned = ticket.Assigned,
                StartDate = ticket.StartDate,
                EndDate = ticket.EndDate,
                ClosedDate = ticket.ClosedDate,
                IsDeleted = ticket.IsDeleted,
                DeletedDate = ticket.DeletedDate
            };
        }
    }
}