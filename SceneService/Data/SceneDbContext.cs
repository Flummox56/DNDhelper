using Microsoft.EntityFrameworkCore;
using SceneService.Models;

namespace SceneService.Data
{
    public class SceneDbContext : DbContext
    {
        public SceneDbContext(DbContextOptions<SceneDbContext> options)
            : base(options)
        {
        }

        public DbSet<Scene> Scenes { get; set; }
        public DbSet<SceneMonster> SceneMonsters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Scene>(entity =>
            {
                entity.ToTable("scenes");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.Reward).HasColumnName("reward");
                entity.Property(e => e.MaxPlayers).HasColumnName("max_players");
                entity.Property(e => e.Status).HasColumnName("status").IsRequired();
                entity.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
                entity.Property(e => e.CreatedByUsername).HasColumnName("created_by_username");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.UpdatedAt).HasColumnName("updated_at");

                entity.HasIndex(e => e.CreatedBy).HasDatabaseName("idx_scenes_created_by");
                entity.HasIndex(e => e.Status).HasDatabaseName("idx_scenes_status");
            });

            modelBuilder.Entity<SceneMonster>(entity =>
            {
                entity.ToTable("scene_monsters");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.SceneId).HasColumnName("scene_id").IsRequired();
                entity.Property(e => e.MonsterId).HasColumnName("monster_id").IsRequired();
                entity.Property(e => e.Name).HasColumnName("name").IsRequired();
                entity.Property(e => e.MaxHP).HasColumnName("max_hp");
                entity.Property(e => e.AC).HasColumnName("ac");
                entity.Property(e => e.Str).HasColumnName("str");
                entity.Property(e => e.Dex).HasColumnName("dex");
                entity.Property(e => e.Con).HasColumnName("con");
                entity.Property(e => e.Int).HasColumnName("int");
                entity.Property(e => e.Wis).HasColumnName("wis");
                entity.Property(e => e.Cha).HasColumnName("cha");
                entity.Property(e => e.Danger).HasColumnName("danger");
                entity.Property(e => e.Description).HasColumnName("description");
                entity.Property(e => e.CreatedBy).HasColumnName("created_by");
                entity.Property(e => e.CreatedByUsername).HasColumnName("created_by_username");
                entity.Property(e => e.Count).HasColumnName("count");

                entity.HasOne(e => e.Scene)
                    .WithMany(s => s.Monsters)
                    .HasForeignKey(e => e.SceneId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => e.SceneId).HasDatabaseName("idx_scene_monsters_scene_id");
            });
        }
    }
}