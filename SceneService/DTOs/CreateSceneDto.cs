namespace SceneService.DTOs
{
    public class CreateSceneDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Reward { get; set; } = string.Empty;
        public int MaxPlayers { get; set; } = 4;
        public string Status { get; set; } = "private";
        public List<CreateSceneMonsterDto> Monsters { get; set; } = new();
    }

    public class CreateSceneMonsterDto
    {
        public string MonsterId { get; set; } = string.Empty;
        public int Count { get; set; } = 1;
    }
}