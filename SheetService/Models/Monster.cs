using System.ComponentModel.DataAnnotations;

namespace SheetService.Models
{
    public class Monster
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

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
        public int Danger { get; set; }

        public string Description { get; set; } = string.Empty;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public string Status { get; set; } = "private";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}