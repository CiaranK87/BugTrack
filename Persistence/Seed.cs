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
                        ProjectOwner = "Project 2 months ago",
                        Description = "This is past project 1",
                        StartDate = DateTime.UtcNow.AddMonths(-2),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                IsOwner = true
                            }
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Past Project 2",
                        ProjectOwner = "Project 1 month ago",
                        Description = "This is past project 1",
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
                        ProjectOwner = "Project 1 months in the future",
                        Description = "This is future project 1",
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
                        ProjectOwner = "Project 2 months in the future",
                        Description = "This is future project 2",
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
                        ProjectOwner = "Project 3 months in the future",
                        Description = "This is future project 3",
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
                        ProjectOwner = "Project 4 months the in future",
                        Description = "This is future project 4",
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
                        ProjectOwner = "Project 5 months in the future",
                        Description = "This is future project 5",
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
                        ProjectOwner = "Project 6 months in the future",
                        Description = "This is future project 6",
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
                        ProjectOwner = "Project 7 months in the future",
                        Description = "This is future project 7",
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
                        ProjectOwner = "Project 8 months in the future",
                        Description = "This is future project 8",
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
