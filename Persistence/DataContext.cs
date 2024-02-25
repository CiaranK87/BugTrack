using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser >
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Project> Projects{ get; set; }
        public DbSet<ProjectParticipant> ProjectParticipants{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProjectParticipant>(x => x.HasKey(pp => new {pp.AppUserId, pp.ProjectId}));

            builder.Entity<ProjectParticipant>()
                .HasOne(pp => pp.AppUser)
                .WithMany(p => p.Projects)
                .HasForeignKey(pp => pp.AppUserId);

            builder.Entity<ProjectParticipant>()
                .HasOne(pp => pp.Project)
                .WithMany(p => p.Participants)
                .HasForeignKey(pp => pp.ProjectId);
        }

    }
}