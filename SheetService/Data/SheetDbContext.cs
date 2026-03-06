using Microsoft.EntityFrameworkCore;
using SheetService.Models;

namespace SheetService.Data
{
    public class SheetDbContext : DbContext
    {
        public SheetDbContext() {}

        public SheetDbContext(DbContextOptions<SheetDbContext> options)
            : base(options)
        {
        }

        public DbSet<CharacterSheet> CharacterSheets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=sheet_db;Username=sheet_user;Password=sheet_password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CharacterSheet>(entity =>
            {
                entity.ToTable("character_sheets");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("uuid");

                entity.Property(e => e.UserId)
                    .HasColumnName("user_id")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.CharacterName)
                    .HasColumnName("character_name")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Race)
                    .HasColumnName("race")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Class)
                    .HasColumnName("class")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("integer");

                entity.Property(e => e.Strength)
                    .HasColumnName("strength")
                    .HasColumnType("integer");

                entity.Property(e => e.Dexterity)
                    .HasColumnName("dexterity")
                    .HasColumnType("integer");

                entity.Property(e => e.Constitution)
                    .HasColumnName("constitution")
                    .HasColumnType("integer");

                entity.Property(e => e.Intelligence)
                    .HasColumnName("intelligence")
                    .HasColumnType("integer");

                entity.Property(e => e.Wisdom)
                    .HasColumnName("wisdom")
                    .HasColumnType("integer");

                entity.Property(e => e.Charisma)
                    .HasColumnName("charisma")
                    .HasColumnType("integer");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.Skills)
                    .HasColumnName("skills")
                    .HasColumnType("jsonb");

                entity.Property(e => e.Inventory)
                    .HasColumnName("inventory")
                    .HasColumnType("jsonb");

                entity.Property(e => e.Spells)
                    .HasColumnName("spells")
                    .HasColumnType("jsonb");

                entity.Property(e => e.Notes)
                    .HasColumnName("notes")
                    .HasColumnType("text");

                // Индекс для быстрого поиска по userId
                entity.HasIndex(e => e.UserId)
                    .HasDatabaseName("idx_character_sheets_user_id");
            });
        }
    }
}