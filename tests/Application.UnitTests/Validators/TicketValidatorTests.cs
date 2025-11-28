namespace Application.UnitTests.Validators
{
    /// <summary>
    /// Tests for ticket validation business rules critical for bug tracking data integrity
    /// </summary>
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
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Business critical validation
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("", "Description here", "submitter", "assigned", "High", "Critical", "Open")] // Empty title
        [InlineData("   ", "Description here", "submitter", "assigned", "High", "Critical", "Open")] // Whitespace title
        [InlineData(null, "Description here", "submitter", "assigned", "High", "Critical", "Open")] // Null title
        public void Validate_InvalidTitle_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Title is critical for bug identification and search
            result.ShouldHaveValidationErrorFor(t => t.Title);
        }

        [Theory]
        [InlineData("Bug title", "", "submitter", "assigned", "High", "Critical", "Open")] // Empty description
        [InlineData("Bug title", "   ", "submitter", "assigned", "High", "Critical", "Open")] // Whitespace description
        [InlineData("Bug title", null, "submitter", "assigned", "High", "Critical", "Open")] // Null description
        public void Validate_InvalidDescription_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Description is critical for understanding and reproducing bugs
            result.ShouldHaveValidationErrorFor(t => t.Description);
        }

        [Theory]
        [InlineData("Bug title", "Description", "", "assigned", "High", "Critical", "Open")] // Empty submitter
        [InlineData("Bug title", "Description", "   ", "assigned", "High", "Critical", "Open")] // Whitespace submitter
        [InlineData("Bug title", "Description", null, "assigned", "High", "Critical", "Open")] // Null submitter
        public void Validate_InvalidSubmitter_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Submitter is critical for audit trail and communication
            result.ShouldHaveValidationErrorFor(t => t.Submitter);
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "", "High", "Critical", "Open")] // Empty assigned
        [InlineData("Bug title", "Description", "submitter", "   ", "High", "Critical", "Open")] // Whitespace assigned
        [InlineData("Bug title", "Description", "submitter", null, "High", "Critical", "Open")] // Null assigned
        public void Validate_InvalidAssigned_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Assigned user is critical for task ownership and tracking
            result.ShouldHaveValidationErrorFor(t => t.Assigned);
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "assigned", "", "Critical", "Open")] // Empty priority
        [InlineData("Bug title", "Description", "submitter", "assigned", "   ", "Critical", "Open")] // Whitespace priority
        [InlineData("Bug title", "Description", "submitter", "assigned", null, "Critical", "Open")] // Null priority
        public void Validate_InvalidPriority_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Priority is critical for resource allocation and triage
            result.ShouldHaveValidationErrorFor(t => t.Priority);
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "", "Open")] // Empty severity
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "   ", "Open")] // Whitespace severity
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", null, "Open")] // Null severity
        public void Validate_InvalidSeverity_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Severity is critical for impact assessment and release planning
            result.ShouldHaveValidationErrorFor(t => t.Severity);
        }

        [Theory]
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "Critical", "")] // Empty status
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "Critical", "   ")] // Whitespace status
        [InlineData("Bug title", "Description", "submitter", "assigned", "High", "Critical", null)] // Null status
        public void Validate_InvalidStatus_ShouldFailBusinessValidation(string title, string description, string submitter, string assigned, string priority, string severity, string status)
        {
            // Arrange
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

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Status is critical for workflow management and reporting
            result.ShouldHaveValidationErrorFor(t => t.Status);
        }

        [Fact]
        public void Validate_DefaultStartDate_ShouldFailBusinessValidation()
        {
            // Arrange
            var ticket = new Ticket
            {
                Title = "Bug title",
                Description = "Description",
                Submitter = "submitter",
                Assigned = "assigned",
                Priority = "High",
                Severity = "Critical",
                Status = "Open",
                StartDate = default(DateTime) // Invalid default date
            };

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - Start date is critical for timeline and SLA tracking
            result.ShouldHaveValidationErrorFor(t => t.StartDate);
        }

        [Fact]
        public void Validate_RealWorldBugScenarios_ShouldPassValidation()
        {
            // Arrange
            var realWorldTickets = new[]
            {
                new
                {
                    Title = "Database connection timeout during peak hours",
                    Description = "Database connections are timing out between 2-4 PM when user traffic is highest. Affects production environment only.",
                    Submitter = "dba@company.com",
                    Assigned = "backend.dev@company.com",
                    Priority = "High",
                    Severity = "Major",
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
                    Assigned = "frontend.dev@company.com",
                    Priority = "Medium",
                    Severity = "Minor",
                    Status = "Resolved"
                }
            };

            // Act & Assert
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

                var result = _validator.TestValidate(ticket);
                result.ShouldNotHaveAnyValidationErrors();
            }
        }

        [Fact]
        public void Validate_CriticalBusinessRules_ShouldEnforceDataIntegrity()
        {
            // Arrange
            var ticket = new Ticket
            {
                Title = "", // Invalid: empty
                Description = "", // Invalid: empty
                Submitter = "", // Invalid: empty
                Assigned = "", // Invalid: empty
                Priority = "", // Invalid: empty
                Severity = "", // Invalid: empty
                Status = "", // Invalid: empty
                StartDate = default(DateTime) // Invalid: default
            };

            // Act
            var result = _validator.TestValidate(ticket);

            // Assert - All critical fields should fail validation
            result.ShouldHaveValidationErrorFor(t => t.Title);
            result.ShouldHaveValidationErrorFor(t => t.Description);
            result.ShouldHaveValidationErrorFor(t => t.Submitter);
            result.ShouldHaveValidationErrorFor(t => t.Assigned);
            result.ShouldHaveValidationErrorFor(t => t.Priority);
            result.ShouldHaveValidationErrorFor(t => t.Severity);
            result.ShouldHaveValidationErrorFor(t => t.Status);
            result.ShouldHaveValidationErrorFor(t => t.StartDate);
        }
    }
}