using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NotificationDatabase.Model
{
    public partial class NotificationServiceContext : DbContext
    {
        public NotificationServiceContext()
        {
        }

        public NotificationServiceContext(DbContextOptions<NotificationServiceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<RegularSendout> RegularSendout { get; set; }
        public virtual DbSet<Templates> Templates { get; set; }
        public virtual DbSet<UserGroups> UserGroups { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.\\sqlexpress;Database=NotificationService;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<RegularSendout>(entity =>
            {

                entity.Property(e => e.DayOfTheWeek)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.LastRunAt).HasColumnType("datetime");

                entity.Property(e => e.Parameters);

                entity.Property(e => e.ReminderName).IsRequired();

                entity.Property(e => e.RepetitionFrequency)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.StartDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<Templates>(entity =>
            {

                entity.Property(e => e.NotificationName).IsRequired();

                entity.Property(e => e.NotificationText).IsRequired();
            });

            modelBuilder.Entity<UserGroups>(entity =>
            {
                entity.Property(e => e.GroupName).IsRequired();
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.Property(e => e.Email).IsRequired();

                entity.Property(e => e.Username).IsRequired();
            });
        }
    }
}
