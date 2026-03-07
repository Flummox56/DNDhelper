namespace SheetService.DTOs
{
    public class MonsterDto
    {
        public Guid Id { get; set; }
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
        public string CreatedBy { get; set; } = string.Empty;
        public string CreatedByUsername { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateMonsterDto
    {
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
        public string Status { get; set; } = "private";
    }
}