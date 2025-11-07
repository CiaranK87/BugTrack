using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Project> Projects{ get; set; }
        public DbSet<ProjectParticipant> ProjectParticipants{ get; set; }
        public DbSet<Ticket> Tickets{ get; set; }
        public DbSet<Comment> Comments{ get; set; }
        public DbSet<CommentAttachment> CommentAttachments{ get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProjectParticipant>(x => x.HasKey(pp => new {pp.AppUserId, pp.ProjectId}));

            builder.Entity<ProjectParticipant>()
                .HasOne(pp => pp.AppUser)
                .WithMany(p => p.ProjectParticipants)
                .HasForeignKey(pp => pp.AppUserId);

            builder.Entity<ProjectParticipant>()
                .HasOne(pp => pp.Project)
                .WithMany(p => p.Participants)
                .HasForeignKey(pp => pp.ProjectId);

            builder.Entity<Comment>()
                .HasOne(c => c.Ticket)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CommentAttachment>()
                .HasOne(ca => ca.Comment)
                .WithMany(c => c.Attachments)
                .HasForeignKey(ca => ca.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CommentAttachment>()
                .HasOne(ca => ca.UploadedBy)
                .WithMany()
                .HasForeignKey(ca => ca.UploadedById)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}