using Microsoft.EntityFrameworkCore;
using SheetService.Models;

namespace SheetService.Data
{
    public class SheetDbContext : DbContext
    {
        public SheetDbContext()
        {
        }

        public SheetDbContext(DbContextOptions<SheetDbContext> options)
            : base(options)
        {
        }

        public DbSet<Monster> Monsters { get; set; } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Port=5434;Database=monster_db;Username=sheet_user;Password=sheet_password");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Monster>(entity =>
            {
                entity.ToTable("monsters");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("uuid");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.MaxHP)
                    .HasColumnName("max_hp")
                    .HasColumnType("integer");

                entity.Property(e => e.AC)
                    .HasColumnName("ac")
                    .HasColumnType("integer");

                entity.Property(e => e.Str)
                    .HasColumnName("str")
                    .HasColumnType("integer");

                entity.Property(e => e.Dex)
                    .HasColumnName("dex")
                    .HasColumnType("integer");

                entity.Property(e => e.Con)
                    .HasColumnName("con")
                    .HasColumnType("integer");

                entity.Property(e => e.Int)
                    .HasColumnName("int")
                    .HasColumnType("integer");

                entity.Property(e => e.Wis)
                    .HasColumnName("wis")
                    .HasColumnType("integer");

                entity.Property(e => e.Cha)
                    .HasColumnName("cha")
                    .HasColumnType("integer");

                entity.Property(e => e.Danger)
                    .HasColumnName("danger")
                    .HasColumnType("integer");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("text");

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("created_by")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Status)
                    .HasColumnName("status")
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("timestamp with time zone");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasColumnType("timestamp with time zone");

                entity.HasIndex(e => e.CreatedBy)
                    .HasDatabaseName("idx_monsters_created_by");

                entity.HasIndex(e => e.Status)
                    .HasDatabaseName("idx_monsters_status");
            });
        }
    }
}