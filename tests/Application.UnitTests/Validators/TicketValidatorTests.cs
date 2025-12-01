using Application.Tickets;
using Domain;
using FluentAssertions;

namespace Application.UnitTests.Validators
{
    public class TicketValidatorTests
    {
        private readonly TicketValidator _validator;

        public TicketValidatorTests()
        {
            _validator = new TicketValidator();
        }

        [Fact]
        public void Validate_CompleteValidTicket_ShouldPassBusinessValidation()
        {
            var ticket = new Ticket
            {
                Title = "Critical authentication failure on mobile app",
                Description = "Users cannot log in using the mobile app when using corporate credentials. Error message: 'Invalid credentials' even with correct password. Desktop version works fine.",
                Submitter = "user@company.com",
                Assigned = "developer@company.com",
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Description here", "submitter", "assigned", "High", "Critical", "Open")]
        [InlineData("   ", "Description here", "submitter", "assigned", "High", "Critical", "Open")]
        [InlineData(null, "Description here", "submitter", "assigned", "High", "Critical", "Open")]
        public void Validate_InvalidTitle_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            var ticket = new Ticket
            {
                Title = title,
                Description = description,
                Submitter = submitter,
                Assigned = assigned,
                Priority = priority,
                Severity = severity,
                Status = status,
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
        }

        [Theory]
        [InlineData("Bug title", "", "submitter", "assigned", "High", "Critical", "Open")]
        [InlineData("Bug title", "   ", "submitter", "assigned", "High", "Critical", "Open")]
        [InlineData("Bug title", null, "submitter", "assigned", "High", "Critical", "Open")]
        public void Validate_InvalidDescription_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            var ticket = new Ticket
            {
                Title = title,
                Description = description,
                Submitter = submitter,
                Assigned = assigned,
                Priority = priority,
                Severity = severity,
                Status = status,
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
        }

        [Theory]
        [InlineData("Bug title", "Description", "", "assigned", "High", "Critical", "Open")]
        [InlineData("Bug title", "Description", "   ", "assigned", "High", "Critical", "Open")]
        [InlineData("Bug title", "Description", null, "assigned", "High", "Critical", "Open")]
        public void Validate_InvalidSubmitter_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            var ticket = new Ticket
            {
                Title = title,
                Description = description,
                Submitter = submitter,
                Assigned = assigned,
                Priority = priority,
                Severity = severity,
                Status = status,
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Submitter");
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "", "High", "Critical", "Open")]
        [InlineData("Bug title", "Description", "submitter", "   ", "High", "Critical", "Open")]
        [InlineData("Bug title", "Description", "submitter", null, "High", "Critical", "Open")]
        public void Validate_InvalidAssigned_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            var ticket = new Ticket
            {
                Title = title,
                Description = description,
                Submitter = submitter,
                Assigned = assigned,
                Priority = priority,
                Severity = severity,
                Status = status,
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Assigned");
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "assigned", "", "Critical", "Open")]
        [InlineData("Bug title", "Description", "submitter", "assigned", "   ", "Critical", "Open")]
        [InlineData("Bug title", "Description", "submitter", "assigned", null, "Critical", "Open")]
        public void Validate_InvalidPriority_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            var ticket = new Ticket
            {
                Title = title,
                Description = description,
                Submitter = submitter,
                Assigned = assigned,
                Priority = priority,
                Severity = severity,
                Status = status,
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Priority");
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "", "Open")]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "   ", "Open")]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", null, "Open")]
        public void Validate_InvalidSeverity_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            var ticket = new Ticket
            {
                Title = title,
                Description = description,
                Submitter = submitter,
                Assigned = assigned,
                Priority = priority,
                Severity = severity,
                Status = status,
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Severity");
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "Critical", "")]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "Critical", "   ")]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "Critical", null)]
        public void Validate_InvalidStatus_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            var ticket = new Ticket
            {
                Title = title,
                Description = description,
                Submitter = submitter,
                Assigned = assigned,
                Priority = priority,
                Severity = severity,
                Status = status,
                StartDate = DateTime.UtcNow
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Status");
        }

        [Fact]
        public void Validate_DefaultStartDate_ShouldFailBusinessValidation()
        {
            var ticket = new Ticket
            {
                Title = "Bug title",
                Description = "Description",
                Submitter = "submitter",
                Assigned = "assigned",
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = default(DateTime)
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "StartDate");
        }

        [Fact]
        public void Validate_RealWorldBugScenarios_ShouldPassValidation()
        {
            var realWorldTickets = new[]
            {
                new
                {
                    Title = "Database connection timeout during peak hours",
                    Description = "Database connections are timing out between 2-4 PM when user traffic is highest. Affects production environment only.",
                    Submitter = "dba@company.com",
                    Assigned = "developer@company.com",
                    Priority = "High",
                    Severity = "Medium",
                    Status = "In Progress"
                },
                new
                {
                    Title = "Mobile app crashes on iOS 16",
                    Description = "App crashes immediately on launch when running on iOS 16 devices. Android version works fine.",
                    Submitter = "user@company.com",
                    Assigned = "developer@company.com",
                    Priority = "Critical",
                    Severity = "Critical",
                    Status = "Open"
                },
                new
                {
                    Title = "CSV export missing data columns",
                    Description = "When exporting reports to CSV, the 'customer_email' and 'phone_number' columns are missing from the output.",
                    Submitter = "support@company.com",
                    Assigned = "developer@company.com",
                    Priority = "Medium",
                    Severity = "Low",
                    Status = "Closed"
                }
            };

            foreach (var ticketData in realWorldTickets)
            {
                var ticket = new Ticket
                {
                    Title = ticketData.Title,
                    Description = ticketData.Description,
                    Submitter = ticketData.Submitter,
                    Assigned = ticketData.Assigned,
                    Priority = ticketData.Priority,
                    Severity = ticketData.Severity,
                    Status = ticketData.Status,
                    StartDate = DateTime.UtcNow
                };

                var result = _validator.Validate(ticket);
                result.IsValid.Should().BeTrue();
            }
        }

        [Fact]
        public void Validate_CriticalBusinessRules_ShouldEnforceDataIntegrity()
        {
            var ticket = new Ticket
            {
                Title = "",
                Description = "",
                Submitter = "",
                Assigned = "",
                Priority = "",
                Severity = "",
                Status = "",
                StartDate = default(DateTime)
            };

            var result = _validator.Validate(ticket);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Title");
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
            result.Errors.Should().Contain(e => e.PropertyName == "Submitter");
            result.Errors.Should().Contain(e => e.PropertyName == "Assigned");
            result.Errors.Should().Contain(e => e.PropertyName == "Priority");
            result.Errors.Should().Contain(e => e.PropertyName == "Severity");
            result.Errors.Should().Contain(e => e.PropertyName == "Status");
            result.Errors.Should().Contain(e => e.PropertyName == "StartDate");
        }
    }
}