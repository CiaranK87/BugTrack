using Application.Projects;
using Domain;
using FluentAssertions;

namespace Application.UnitTests.Validators
{
    public class ProjectValidatorTests
    {
        private readonly ProjectValidator _validator;

        public ProjectValidatorTests()
        {
            _validator = new ProjectValidator();
        }

        [Fact]
        public void Validate_CompleteValidProject_ShouldPassBusinessValidation()
        {
            var project = new Project
            {
                ProjectTitle = "Customer Portal Enhancement Phase 2",
                Description = "Second phase of customer portal enhancement including new dashboard, advanced search functionality, and mobile responsiveness improvements. This project addresses customer feedback from phase 1 and aims to increase user engagement by 25%.",
                StartDate = DateTime.UtcNow.AddDays(14)
            };

            var result = _validator.Validate(project);

            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("", "Description here", "2023-01-01")]
        [InlineData("   ", "Description here", "2023-01-01")]
        [InlineData(null, "Description here", "2023-01-01")]
        public void Validate_InvalidProjectTitle_ShouldFailBusinessValidation(string projectTitle, string description, string startDateString)
        {
            var project = new Project
            {
                ProjectTitle = projectTitle,
                Description = description,
                StartDate = DateTime.Parse(startDateString)
            };

            var result = _validator.Validate(project);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ProjectTitle");
        }

        [Theory]
        [InlineData("Project Title", "", "2023-01-01")]
        [InlineData("Project Title", "   ", "2023-01-01")]
        [InlineData("Project Title", null, "2023-01-01")]
        public void Validate_InvalidDescription_ShouldFailBusinessValidation(string projectTitle, string description, string startDateString)
        {
            var project = new Project
            {
                ProjectTitle = projectTitle,
                Description = description,
                StartDate = DateTime.Parse(startDateString)
            };

            var result = _validator.Validate(project);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
        }

        [Fact]
        public void Validate_DefaultStartDate_ShouldFailBusinessValidation()
        {
            var project = new Project
            {
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = default(DateTime)
            };

            var result = _validator.Validate(project);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "StartDate");
        }

        [Fact]
        public void Validate_RealWorldProjectScenarios_ShouldPassValidation()
        {
            var realWorldProjects = new[]
            {
                new
                {
                    Title = "API Gateway Implementation",
                    Description = "Implement API gateway to handle authentication, rate limiting, and routing for all microservices. This will improve security and performance across the platform.",
                    StartDate = DateTime.UtcNow.AddDays(30)
                },
                new
                {
                    Title = "Mobile App Development",
                    Description = "Develop native mobile applications for iOS and Android platforms to provide better user experience and offline capabilities.",
                    StartDate = DateTime.UtcNow.AddDays(60)
                },
                new
                {
                    Title = "Database Migration to Cloud",
                    Description = "Migrate on-premise databases to cloud-based solution for better scalability, performance, and disaster recovery capabilities.",
                    StartDate = DateTime.UtcNow.AddDays(90)
                },
                new
                {
                    Title = "Security Audit and Remediation",
                    Description = "Comprehensive security audit of all systems and implementation of recommended security improvements to meet compliance requirements.",
                    StartDate = DateTime.UtcNow.AddDays(7)
                }
            };

            foreach (var projectData in realWorldProjects)
            {
                var project = new Project
                {
                    ProjectTitle = projectData.Title,
                    Description = projectData.Description,
                    StartDate = projectData.StartDate
                };

                var result = _validator.Validate(project);
                result.IsValid.Should().BeTrue();
            }
        }

        [Theory]
        [InlineData("A", "Single character project title", "2023-01-01")] // Very short title
        [InlineData("This is a very long project title that exceeds normal length but should still be valid according to current validation rules", "Description", "2023-01-01")] // Very long title
        [InlineData("Project", "Short description", "2023-01-01")] // Short description
        [InlineData("Project", "This is a very long project description that provides detailed information about the project goals, objectives, scope, timeline, and other important details that stakeholders need to understand", "2023-01-01")] // Long description
        public void Validate_VariousContentLengths_ShouldPassValidation(string projectTitle, string description, string startDateString)
        {
            var project = new Project
            {
                ProjectTitle = projectTitle,
                Description = description,
                StartDate = DateTime.Parse(startDateString)
            };

            var result = _validator.Validate(project);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("2023-01-01")]
        [InlineData("2024-12-31")]
        [InlineData("2025-06-15")]
        public void Validate_TimelineValidation_ShouldPassValidation(string startDateString)
        {
            var project = new Project
            {
                ProjectTitle = "Test Project",
                Description = "Test Description",
                StartDate = DateTime.Parse(startDateString)
            };

            var result = _validator.Validate(project);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_CriticalBusinessRules_ShouldEnforceDataIntegrity()
        {
            var project = new Project
            {
                ProjectTitle = "",
                Description = "",
                StartDate = default(DateTime)
            };

            var result = _validator.Validate(project);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ProjectTitle");
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
            result.Errors.Should().Contain(e => e.PropertyName == "StartDate");
        }

        [Fact]
        public void Validate_ProjectPlanningScenarios_ShouldSupportRealBusinessNeeds()
        {
            var businessProjects = new[]
            {
                new
                {
                    Title = "Q4 Feature Release",
                    Description = "Release new features for Q4 including customer dashboard, reporting improvements, and mobile app enhancements",
                    StartDate = new DateTime(2023, 10, 1),
                    BusinessContext = "Quarterly release cycle"
                },
                new
                {
                    Title = "Compliance Update",
                    Description = "Update systems to meet new GDPR and CCPA compliance requirements including data handling and privacy controls",
                    StartDate = new DateTime(2023, 8, 15),
                    BusinessContext = "Regulatory compliance"
                },
                new
                {
                    Title = "Performance Optimization",
                    Description = "Optimize application performance to handle 2x current load and improve response times by 40%",
                    StartDate = new DateTime(2023, 9, 1),
                    BusinessContext = "Scalability requirements"
                }
            };

            foreach (var projectData in businessProjects)
            {
                var project = new Project
                {
                    ProjectTitle = projectData.Title,
                    Description = projectData.Description,
                    StartDate = projectData.StartDate
                };

                var result = _validator.Validate(project);
                result.IsValid.Should().BeTrue();
            }
        }

        [Fact]
        public void Validate_ProjectTimelineValidation_ShouldSupportPlanning()
        {
            var today = DateTime.UtcNow;
            var timelineScenarios = new[]
            {
                new { Title = "Immediate Start", StartDateString = "2023-01-01", Context = "Urgent project" },
                new { Title = "Next Week", StartDateString = "2023-01-08", Context = "Standard planning" },
                new { Title = "Next Month", StartDateString = "2023-02-01", Context = "Monthly planning cycle" },
                new { Title = "Next Quarter", StartDateString = "2023-04-01", Context = "Quarterly planning" }
            };

            foreach (var scenario in timelineScenarios)
            {
                var project = new Project
                {
                    ProjectTitle = scenario.Title,
                    Description = $"Project starting {scenario.Context}",
                    StartDate = DateTime.Parse(scenario.StartDateString)
                };

                var result = _validator.Validate(project);
                result.IsValid.Should().BeTrue();
            }
        }
    }
}