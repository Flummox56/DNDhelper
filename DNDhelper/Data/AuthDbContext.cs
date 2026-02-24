using Microsoft.EntityFrameworkCore;
using DNDhelper.Models;

namespace DNDhelper.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext()
        {
        }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=auth_db;Username=auth_user;Password=auth_password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Session>().ToTable("sessions");

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("text");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.HasKey(e => e.SessionId);

                entity.Property(e => e.SessionId)
                    .HasColumnName("session_id")
                    .HasColumnType("text");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.RefreshToken)
                    .HasColumnName("refresh_token")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.IpAddress)
                    .HasColumnName("ip_address")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.UserAgent)
                    .HasColumnName("user_agent")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.ExpiredAt)
                    .HasColumnName("expired_at")
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                entity.HasOne(e => e.User)
                    .WithMany(e => e.Sessions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (!entity.GetTableName().StartsWith("__EF"))
                {
                    entity.SetTableName(entity.GetTableName().ToLower());
                }

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToLower());
                }
            }
        }
    }
}