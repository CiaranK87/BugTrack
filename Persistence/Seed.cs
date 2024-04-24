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
                        ProjectOwner = "Bob",
                        Description = "This is past project 1 description",
                        StartDate = DateTime.UtcNow.AddMonths(-2),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                IsOwner = true
                            }                            
                        },
                        Tickets = new List<Ticket>
                        {
                            new Ticket
                            {
                                Title = "Ticket 1",
                                Description = "Ticket 1 description",
                                Submitter = "Ticket 1 submitter",
                                Assigned = "Ticket 1 assigned",
                                priority = "Ticket 1 priority",
                                Severity = "Ticket 1 severity",
                                Status = "Ticket 1 status",
                                StartDate = DateTime.UtcNow,
                                Updated = DateTime.UtcNow,
                            },
                            new Ticket
                            {
                                Title = "Ticket 2",
                                Description = "Ticket 2 description",
                                Submitter = "Ticket 2 submitter",
                                Assigned = "Ticket 2 assigned",
                                priority = "Ticket 2 priority",
                                Severity = "Ticket 2 severity",
                                Status = "Ticket 2 status",
                                StartDate = DateTime.UtcNow,
                                Updated = DateTime.UtcNow,
                            }
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Past Project 2",
                        ProjectOwner = "Tim",
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
                        ProjectOwner = "Jane",
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
                        ProjectOwner = "Bob",
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
                    new Project
                    {
                        ProjectTitle = "Future Project 3",
                        ProjectOwner = "Tim",
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
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Future Project 4",
                        ProjectOwner = "Jane",
                        Description = "This is future project 4 description",
                        StartDate = DateTime.UtcNow.AddMonths(4),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[1],
                                IsOwner = true                            
                            },
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Future Project 5",
                        ProjectOwner = "Bob",
                        Description = "This is future project 5 description",
                        StartDate = DateTime.UtcNow.AddMonths(5),
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
                        ProjectTitle = "Future Project 6",
                        ProjectOwner = "Jane",
                        Description = "This is future project 6 description",
                        StartDate = DateTime.UtcNow.AddMonths(6),
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
                        ProjectTitle = "Future Project 7",
                        ProjectOwner = "Bob",
                        Description = "This is future project 7 description",
                        StartDate = DateTime.UtcNow.AddMonths(7),
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
                    new Project
                    {
                        ProjectTitle = "Future Project 8",
                        ProjectOwner = "Tim",
                        Description = "This is future project 8 description",
                        StartDate = DateTime.UtcNow.AddMonths(8),
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
                    }
                };

                await context.Projects.AddRangeAsync(projects);
                await context.SaveChangesAsync();
            }
        }
    }
}
