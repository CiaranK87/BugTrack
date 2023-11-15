using Domain;

namespace Persistence
{
    public class Seed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Projects.Any()) return;
            
            var projects = new List<Project>
            {
                new Project
                {
                    Name = "Seed Project 1",
                    ProjectOwner = "John",
                    Description = "Seed Project 1 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(1),
                },
		new Project
                {
                    Name = "Seed Project 2",
                    ProjectOwner = "Tom",
                    Description = "Seed Project 2 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(2),
                },
		new Project
                {
                    Name = "Seed Project 3",
                    ProjectOwner = "Sarah",
                    Description = "Seed Project 3 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(3),
                },
		new Project
                {
                    Name = "Seed Project 4",
                    ProjectOwner = "Sally",
                    Description = "Seed Project 4 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(4),
                },
		new Project
                {
                    Name = "Seed Project 5",
                    ProjectOwner = "Mike",
                    Description = "Seed Project 5 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(5),
                },
		new Project
                {
                    Name = "Seed Project 6",
                    ProjectOwner = "Susan",
                    Description = "Seed Project 6 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(6),
                },
		new Project
                {
                    Name = "Seed Project 7",
                    ProjectOwner = "Paul",
                    Description = "Seed Project 7 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(7),
                },
		new Project
                {
                    Name = "Seed Project 8",
                    ProjectOwner = "Tammy",
                    Description = "Seed Project 8 Description",
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMonths(8),
                },
            };

            await context.Projects.AddRangeAsync(projects);
            await context.SaveChangesAsync();
        }
    }
}