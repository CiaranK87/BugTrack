using Domain;
using Microsoft.AspNetCore.Identity;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser{DisplayName = "Bob", UserName= "bob", Email = "bob@test.com"},
                    new AppUser{DisplayName = "Tim", UserName= "tim", Email = "tim@test.com"},
                    new AppUser{DisplayName = "Jim", UserName= "jim", Email = "jim@test.com"},
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$w0rd");
                }
            }



            if (context.Projects.Any()) return;
            
            var projects = new List<Project>
            {
                new Project
                {
                    Name = "Seed Project 1",
                    ProjectOwner = "John",
                    Description = "Seed Project 1 Description",
                    StartDate = DateTime.UtcNow,
                },
		new Project
                {
                    Name = "Seed Project 2",
                    ProjectOwner = "Tom",
                    Description = "Seed Project 2 Description",
                    StartDate = DateTime.UtcNow,
                },
		new Project
                {
                    Name = "Seed Project 3",
                    ProjectOwner = "Sarah",
                    Description = "Seed Project 3 Description",
                    StartDate = DateTime.UtcNow,
                },
		new Project
                {
                    Name = "Seed Project 4",
                    ProjectOwner = "Sally",
                    Description = "Seed Project 4 Description",
                    StartDate = DateTime.UtcNow,
                },
		new Project
                {
                    Name = "Seed Project 5",
                    ProjectOwner = "Mike",
                    Description = "Seed Project 5 Description",
                    StartDate = DateTime.UtcNow,
                },
		new Project
                {
                    Name = "Seed Project 6",
                    ProjectOwner = "Susan",
                    Description = "Seed Project 6 Description",
                    StartDate = DateTime.UtcNow,
                },
		new Project
                {
                    Name = "Seed Project 7",
                    ProjectOwner = "Paul",
                    Description = "Seed Project 7 Description",
                    StartDate = DateTime.UtcNow,
                },
		new Project
                {
                    Name = "Seed Project 8",
                    ProjectOwner = "Tammy",
                    Description = "Seed Project 8 Description",
                    StartDate = DateTime.UtcNow,
                },
            };

            await context.Projects.AddRangeAsync(projects);
            await context.SaveChangesAsync();
        }
    }
}