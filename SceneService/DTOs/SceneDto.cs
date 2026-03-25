namespace SceneService.DTOs
{
    public class SceneDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reward { get; set; } = string.Empty;
        public int MaxPlayers { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUsername { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<SceneMonsterDto> Monsters { get; set; } = new();
    }

    public class SceneMonsterDto
    {
        public Guid Id { get; set; }
        public string MonsterId { get; set; } = string.Empty;
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
        public int Count { get; set; }
    }
}