using System.ComponentModel.DataAnnotations;

namespace SheetService.Models
{
    public class CharacterSheet
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string CharacterName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Race { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Class { get; set; } = string.Empty;

        public int Level { get; set; } = 1;

        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Constitution { get; set; }
        public int Intelligence { get; set; }
        public int Wisdom { get; set; }
        public int Charisma { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string? Skills { get; set; } // JSON
        public string? Inventory { get; set; } // JSON
        public string? Spells { get; set; } // JSON
        public string? Notes { get; set; }
    }
}