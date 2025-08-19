using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
            UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any() && !context.Projects.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser
                    {
                        DisplayName = "Bob",
                        UserName = "bob",
                        Email = "bob@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Jane",
                        UserName = "jane",
                        Email = "jane@test.com"
                    },
                    new AppUser
                    {
                        DisplayName = "Tim",
                        UserName = "tim",
                        Email = "tim@test.com"
                    },
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }

                var projects = new List<Project>
                {
                    new Project
                    {
                        ProjectTitle = "Past Project 1",
                        ProjectOwner = "bob", // ✅ username, not display name
                        Description = "This is past project 1 description",
                        StartDate = DateTime.UtcNow.AddMonths(-2),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[1],
                                IsOwner = false
                            }
                        },
                        Tickets = new List<Ticket>
                        {
                            new Ticket
                            {
                                Title = "Ticket 1",
                                Description = "Ticket 1 description",
                                Submitter = "bob", // ✅ real username
                                Assigned = "jane", // ✅ real username
                                Priority = "High",
                                Severity = "Critical",
                                Status = "In Progress",
                                StartDate = DateTime.UtcNow.AddDays(-1),
                                EndDate = DateTime.UtcNow.AddDays(2),
                                Updated = DateTime.UtcNow
                            },
                            new Ticket
                            {
                                Title = "Ticket 2",
                                Description = "Ticket 2 description",
                                Submitter = "jane",
                                Assigned = "bob", // ✅ assign to bob
                                Priority = "Medium",
                                Severity = "Medium",
                                Status = "Pending",
                                StartDate = DateTime.UtcNow,
                                EndDate = DateTime.UtcNow.AddDays(5),
                                Updated = DateTime.UtcNow
                            }
                        }
                    },
                    // Other projects — already good, but ensure ProjectOwner = username
                    new Project
                    {
                        ProjectTitle = "Past Project 2",
                        ProjectOwner = "tim",
                        Description = "This is past project 2 description ",
                        StartDate = DateTime.UtcNow.AddMonths(-1),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                IsOwner = true                            
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[1],
                                IsOwner = false                            
                            },
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Future Project 1",
                        ProjectOwner = "jane",
                        Description = "This is future project 1 description",
                        StartDate = DateTime.UtcNow.AddMonths(1),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[2],
                                IsOwner = true                            
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[1],
                                IsOwner = false                            
                            },
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Future Project 2",
                        ProjectOwner = "bob",
                        Description = "This is future project 2 description",
                        StartDate = DateTime.UtcNow.AddMonths(2),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[2],
                                IsOwner = false
                            },
                        }
                    },
                    // Keep rest similar — just fix ProjectOwner to use username
                    // Example:
                    new Project
                    {
                        ProjectTitle = "Future Project 3",
                        ProjectOwner = "tim",
                        Description = "This is future project 3 description",
                        StartDate = DateTime.UtcNow.AddMonths(3),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[1],
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                IsOwner = false
                            },
                        },
                        Tickets = new List<Ticket>
                        {
                            new Ticket
                            {
                                Title = "Ticket 1",
                                Description = "Ticket 1 description",
                                Submitter = "tim",
                                Assigned = "bob",
                                Priority = "Low",
                                Severity = "Low",
                                Status = "Open",
                                StartDate = DateTime.UtcNow,
                                EndDate = DateTime.UtcNow.AddDays(10),
                                Updated = DateTime.UtcNow,
                            }
                        }
                    },
                    // Add 1-2 more projects with tickets if needed
                };

                await context.Projects.AddRangeAsync(projects);
                await context.SaveChangesAsync();
            }
        }
    }
}