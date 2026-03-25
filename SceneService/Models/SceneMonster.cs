using System.ComponentModel.DataAnnotations;

namespace SceneService.Models
{
    public class SceneMonster
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid SceneId { get; set; }

        [Required]
        public string MonsterId { get; set; } = string.Empty;

        [Required]
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

        public int Count { get; set; } = 1;

        public Scene Scene { get; set; } = null!;
    }
}