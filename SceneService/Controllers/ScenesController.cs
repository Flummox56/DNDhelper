using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SceneService.Data;
using SceneService.Models;
using SceneService.DTOs;

namespace SceneService.Controllers
{
    [Route("api/scenes")]
    [ApiController]
    public class ScenesController : ControllerBase
    {
        private readonly SceneDbContext _context;
        private readonly MonsterReadDbContext _monsterContext;

        public ScenesController(SceneDbContext context, MonsterReadDbContext monsterContext)
        {
            _context = context;
            _monsterContext = monsterContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SceneDto>>> GetScenes()
        {
            var scenes = await _context.Scenes
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();

            var result = scenes.Select(s => new SceneDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Reward = s.Reward,
                MaxPlayers = s.MaxPlayers,
                Status = s.Status,
                CreatedBy = s.CreatedBy,
                CreatedByUsername = s.CreatedByUsername,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,
                Monsters = new List<SceneMonsterDto>()
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SceneDto>> GetScene(Guid id)
        {
            var scene = await _context.Scenes
                .Include(s => s.Monsters)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scene == null)
                return NotFound();

            var result = new SceneDto
            {
                Id = scene.Id,
                Name = scene.Name,
                Description = scene.Description,
                Reward = scene.Reward,
                MaxPlayers = scene.MaxPlayers,
                Status = scene.Status,
                CreatedBy = scene.CreatedBy,
                CreatedByUsername = scene.CreatedByUsername,
                CreatedAt = scene.CreatedAt,
                UpdatedAt = scene.UpdatedAt,
                Monsters = scene.Monsters.Select(m => new SceneMonsterDto
                {
                    Id = m.Id,
                    MonsterId = m.MonsterId,
                    Name = m.Name,
                    MaxHP = m.MaxHP,
                    AC = m.AC,
                    Str = m.Str,
                    Dex = m.Dex,
                    Con = m.Con,
                    Int = m.Int,
                    Wis = m.Wis,
                    Cha = m.Cha,
                    Danger = m.Danger,
                    Description = m.Description,
                    Count = m.Count
                }).ToList()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SceneDto>> CreateScene(CreateSceneDto createDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "temp-user";
            var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";

            var scene = new Scene
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Reward = createDto.Reward,
                MaxPlayers = createDto.MaxPlayers,
                Status = createDto.Status,
                CreatedBy = userId,
                CreatedByUsername = username,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            foreach (var monsterRef in createDto.Monsters)
            {
                var monsterInfo = await _monsterContext.Monsters
                    .FirstOrDefaultAsync(m => m.Id == monsterRef.MonsterId);

                if (monsterInfo == null)
                    continue;

                scene.Monsters.Add(new SceneMonster
                {
                    MonsterId = monsterInfo.Id,
                    Name = monsterInfo.Name,
                    MaxHP = monsterInfo.MaxHP,
                    AC = monsterInfo.AC,
                    Str = monsterInfo.Str,
                    Dex = monsterInfo.Dex,
                    Con = monsterInfo.Con,
                    Int = monsterInfo.Int,
                    Wis = monsterInfo.Wis,
                    Cha = monsterInfo.Cha,
                    Danger = monsterInfo.Danger,
                    Description = monsterInfo.Description,
                    CreatedBy = monsterInfo.CreatedBy,
                    CreatedByUsername = monsterInfo.CreatedByUsername,
                    Count = monsterRef.Count
                });
            }

            _context.Scenes.Add(scene);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetScene), new { id = scene.Id }, await MapToDto(scene));
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<SceneDto>> UpdateScene(Guid id, CreateSceneDto updateDto)
        {
            var scene = await _context.Scenes
                .Include(s => s.Monsters)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (scene == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "temp-user";
            if (scene.CreatedBy != userId)
                return Forbid();

            scene.Name = updateDto.Name;
            scene.Description = updateDto.Description;
            scene.Reward = updateDto.Reward;
            scene.MaxPlayers = updateDto.MaxPlayers;
            scene.Status = updateDto.Status;
            scene.UpdatedAt = DateTime.UtcNow;

            _context.SceneMonsters.RemoveRange(scene.Monsters);
            scene.Monsters.Clear();

            foreach (var monsterRef in updateDto.Monsters)
            {
                var monsterInfo = await _monsterContext.Monsters
                    .FirstOrDefaultAsync(m => m.Id == monsterRef.MonsterId);

                if (monsterInfo == null)
                    continue;

                scene.Monsters.Add(new SceneMonster
                {
                    MonsterId = monsterInfo.Id,
                    Name = monsterInfo.Name,
                    MaxHP = monsterInfo.MaxHP,
                    AC = monsterInfo.AC,
                    Str = monsterInfo.Str,
                    Dex = monsterInfo.Dex,
                    Con = monsterInfo.Con,
                    Int = monsterInfo.Int,
                    Wis = monsterInfo.Wis,
                    Cha = monsterInfo.Cha,
                    Danger = monsterInfo.Danger,
                    Description = monsterInfo.Description,
                    CreatedBy = monsterInfo.CreatedBy,
                    CreatedByUsername = monsterInfo.CreatedByUsername,
                    Count = monsterRef.Count
                });
            }

            await _context.SaveChangesAsync();

            return Ok(await MapToDto(scene));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScene(Guid id)
        {
            var scene = await _context.Scenes.FindAsync(id);
            if (scene == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "temp-user";
            if (scene.CreatedBy != userId)
                return Forbid();

            _context.Scenes.Remove(scene);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<SceneDto> MapToDto(Scene scene)
        {
            return new SceneDto
            {
                Id = scene.Id,
                Name = scene.Name,
                Description = scene.Description,
                Reward = scene.Reward,
                MaxPlayers = scene.MaxPlayers,
                Status = scene.Status,
                CreatedBy = scene.CreatedBy,
                CreatedByUsername = scene.CreatedByUsername,
                CreatedAt = scene.CreatedAt,
                UpdatedAt = scene.UpdatedAt,
                Monsters = scene.Monsters.Select(m => new SceneMonsterDto
                {
                    Id = m.Id,
                    MonsterId = m.MonsterId,
                    Name = m.Name,
                    MaxHP = m.MaxHP,
                    AC = m.AC,
                    Str = m.Str,
                    Dex = m.Dex,
                    Con = m.Con,
                    Int = m.Int,
                    Wis = m.Wis,
                    Cha = m.Cha,
                    Danger = m.Danger,
                    Description = m.Description,
                    Count = m.Count
                }).ToList()
            };
        }
    }
}