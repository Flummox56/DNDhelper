using System.ComponentModel.DataAnnotations;

namespace SceneService.Models
{
    public class Scene
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Reward { get; set; } = string.Empty;

        public int MaxPlayers { get; set; } = 4;

        public string Status { get; set; } = "private";

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string CreatedByUsername { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<SceneMonster> Monsters { get; set; } = new List<SceneMonster>();
    }
}