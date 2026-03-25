using Microsoft.EntityFrameworkCore;

namespace SceneService.Data
{
    public class MonsterReadDbContext : DbContext
    {
        public MonsterReadDbContext(DbContextOptions<MonsterReadDbContext> options)
            : base(options)
        {
        }

        public DbSet<MonsterInfo> Monsters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MonsterInfo>(entity =>
            {
                entity.ToTable("monsters");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("id");
                entity.Property(e => e.Name).HasColumnName("name");
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
            });
        }
    }

    public class MonsterInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int MaxHP { get; set; }
        public int AC { get; set; }
        public int Str { get; set; }
        public int Dex { get; set; }
        public int Con { get; set; }
        public int Int { get; set; }
        public int Wis { get; set; }
        public int Cha { get; set; }
        public double Danger { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUsername { get; set; } = string.Empty;
    }
}