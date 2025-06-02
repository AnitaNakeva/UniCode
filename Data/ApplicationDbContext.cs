using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UniCodeProject.API.DataModels;

namespace UniCodeProject.API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskModel>()
            .HasKey(t => t.Id);

            modelBuilder.Entity<LecturerEmailDomain>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Domain).IsRequired();
            });

            modelBuilder.Entity<StudentEmailDomain>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Domain).IsRequired();
            });

            modelBuilder.Entity<StudentProfile>()
                .HasOne(sp => sp.User)
                .WithMany()
                .HasForeignKey(sp => sp.UserId);

            modelBuilder.Entity<TaskStudent>()
                .HasKey(ts => new { ts.TaskId, ts.StudentProfileId });

            modelBuilder.Entity<TaskStudent>()
                .HasOne(ts => ts.Task)
                .WithMany()
                .HasForeignKey(ts => ts.TaskId);

            modelBuilder.Entity<TaskStudent>()
                .HasOne(ts => ts.StudentProfile)
                .WithMany(sp => sp.TaskStudents)
                .HasForeignKey(ts => ts.StudentProfileId);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Lecturer)
                .WithMany()
                .HasForeignKey(t => t.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskSubmission>()
                .HasOne(ts => ts.Task)
                .WithMany()
                .HasForeignKey(ts => ts.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskSubmission>()
                .HasOne(ts => ts.Student)
                .WithMany()
                .HasForeignKey(ts => ts.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailConfirmationToken>().HasKey(e => e.Id);

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<LecturerEmailDomain> LecturerEmailDomains { get; set; } = null!;
        public DbSet<StudentEmailDomain> StudentEmailDomains { get; set; } = null!;
        public DbSet<StudentProfile> StudentProfiles { get; set; } = null!;
        public DbSet<LecturerProfile> LecturerProfiles { get; set; } = null!;
        public DbSet<TaskModel> TaskModels { get; set; } = null!;
        public DbSet<TaskStudent> TaskStudents { get; set; } = null!;
        public DbSet<TaskSubmission> TaskSubmissions { get; set; } = null!;
        public DbSet<EmailConfirmationToken> EmailConfirmationTokens { get; set; }

    }

}
