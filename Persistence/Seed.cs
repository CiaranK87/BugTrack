using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            if (!userManager.Users.Any() && !context.Projects.Any())
            {
                if (!await roleManager.RoleExistsAsync("Admin"))
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (!await roleManager.RoleExistsAsync("ProjectManager"))
                    await roleManager.CreateAsync(new IdentityRole("ProjectManager"));
                if (!await roleManager.RoleExistsAsync("Developer"))
                    await roleManager.CreateAsync(new IdentityRole("Developer"));
                if (!await roleManager.RoleExistsAsync("User"))
                    await roleManager.CreateAsync(new IdentityRole("User"));


        var users = new List<AppUser>
            {
                new AppUser { DisplayName = "Bob", UserName = "bob", Email = "bob@test.com" },
                new AppUser { DisplayName = "Jane", UserName = "jane", Email = "jane@test.com" },
                new AppUser { DisplayName = "Tim", UserName = "tim", Email = "tim@test.com" },
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }

            var bob = await userManager.FindByEmailAsync("bob@test.com");
            if (bob != null && !await userManager.IsInRoleAsync(bob, "ProjectManager"))
            {
                await userManager.AddToRoleAsync(bob, "ProjectManager");
                bob.GlobalRole = "ProjectManager";
                await userManager.UpdateAsync(bob);
            }

            var jane = await userManager.FindByEmailAsync("jane@test.com");
            if (jane != null && !await userManager.IsInRoleAsync(jane, "Admin"))
            {
                await userManager.AddToRoleAsync(jane, "Admin");
                jane.GlobalRole = "Admin";
                await userManager.UpdateAsync(jane);
            }

            var tim = await userManager.FindByEmailAsync("tim@test.com");
            if (tim != null && !await userManager.IsInRoleAsync(tim, "User"))
            {
                await userManager.AddToRoleAsync(tim, "User");
                tim.GlobalRole = "User";
                await userManager.UpdateAsync(tim);
            }

                var projects = new List<Project>
                {
                    new Project
                    {
                        ProjectTitle = "Past Project 1",
                        Description = "This is past project 1 description",
                        ProjectOwner = "bob",
                        StartDate = DateTime.UtcNow.AddMonths(-2),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                Role = "Owner",
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[2],
                                Role = "Developer",
                                IsOwner = false
                            }
                        },
                        Tickets = new List<Ticket>
                        {
                            new Ticket
                            {
                                Title = "Ticket 1",
                                Description = "Ticket 1 description",
                                Submitter = "bob", 
                                Assigned = "jane",
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
                                Assigned = "bob",
                                Priority = "Medium",
                                Severity = "Medium",
                                Status = "Open",
                                StartDate = DateTime.UtcNow,
                                EndDate = DateTime.UtcNow.AddDays(5),
                                Updated = DateTime.UtcNow
                            }
                        }
                    },
                    
                    new Project
                    {
                        ProjectTitle = "Past Project 2",
                        Description = "This is past project 2 description ",
                        ProjectOwner = "tim",
                        StartDate = DateTime.UtcNow.AddMonths(-1),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[2],
                                Role = "Owner",
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[1],
                                Role = "Developer",
                                IsOwner = false
                            },
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Future Project 1",
                        Description = "This is future project 1 description",
                        ProjectOwner = "jane",
                        StartDate = DateTime.UtcNow.AddMonths(1),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[1],
                                Role = "Owner",
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[2],
                                Role = "Developer",
                                IsOwner = false
                            },
                        }
                    },
                    new Project
                    {
                        ProjectTitle = "Future Project 2",
                        Description = "This is future project 2 description",
                        ProjectOwner = "bob",
                        StartDate = DateTime.UtcNow.AddMonths(2),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                Role = "Owner",
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[2],
                                Role = "Developer",
                                IsOwner = false
                            },
                        }
                    },
                    
                    
                    new Project
                    {
                        ProjectTitle = "Future Project 3",
                        Description = "This is future project 3 description",
                        ProjectOwner = "tim",
                        StartDate = DateTime.UtcNow.AddMonths(3),
                        Participants = new List<ProjectParticipant>
                        {
                            new ProjectParticipant
                            {
                                AppUser = users[2],
                                Role = "Owner",
                                IsOwner = true
                            },
                            new ProjectParticipant
                            {
                                AppUser = users[0],
                                Role = "Developer",
                                IsOwner = false
                            },
                        },
                        Tickets = new List<Ticket>
                        {
                            new Ticket
                            {
                                Title = "Ticket 3",
                                Description = "Ticket 3 description",
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
                    
                };

                await context.Projects.AddRangeAsync(projects);
                await context.SaveChangesAsync();
            }
        }
    }
}
