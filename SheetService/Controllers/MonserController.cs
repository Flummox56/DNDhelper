using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SheetService.Data;
using SheetService.Models;
using SheetService.DTOs;
using System.Text.Json;

namespace SheetService.Controllers
{
    [Route("api/monsters")]
    [ApiController]
    [Authorize]
    public class MonstersController : ControllerBase
    {
        private readonly SheetDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public MonstersController(SheetDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonsterDto>>> GetMonsters()
        {
            var monsters = await _context.Monsters
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            var creatorIds = monsters.Select(m => m.CreatedBy).Distinct().ToList();
            var usernames = await GetUsernamesFromGateway(creatorIds);

            var result = monsters.Select(m => new MonsterDto
            {
                Id = m.Id,
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
                CreatedBy = m.CreatedBy,
                CreatedByUsername = usernames.ContainsKey(m.CreatedBy) ? usernames[m.CreatedBy] : "Unknown",
                Status = m.Status,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            });

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MonsterDto>> GetMonster(Guid id)
        {
            var monster = await _context.Monsters.FindAsync(id);

            if (monster == null)
            {
                return NotFound();
            }

            var username = await GetUsernameFromGateway(monster.CreatedBy);

            var result = new MonsterDto
            {
                Id = monster.Id,
                Name = monster.Name,
                MaxHP = monster.MaxHP,
                AC = monster.AC,
                Str = monster.Str,
                Dex = monster.Dex,
                Con = monster.Con,
                Int = monster.Int,
                Wis = monster.Wis,
                Cha = monster.Cha,
                Danger = monster.Danger,
                Description = monster.Description,
                CreatedBy = monster.CreatedBy,
                CreatedByUsername = username,
                Status = monster.Status,
                CreatedAt = monster.CreatedAt,
                UpdatedAt = monster.UpdatedAt
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<MonsterDto>> CreateMonster(CreateMonsterDto createDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var monster = new Monster
            {
                Name = createDto.Name,
                MaxHP = createDto.MaxHP,
                AC = createDto.AC,
                Str = createDto.Str,
                Dex = createDto.Dex,
                Con = createDto.Con,
                Int = createDto.Int,
                Wis = createDto.Wis,
                Cha = createDto.Cha,
                Danger = createDto.Danger,
                Description = createDto.Description,
                CreatedBy = userId,
                Status = createDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Monsters.Add(monster);
            await _context.SaveChangesAsync();

            var username = await GetUsernameFromGateway(userId);

            var result = new MonsterDto
            {
                Id = monster.Id,
                Name = monster.Name,
                MaxHP = monster.MaxHP,
                AC = monster.AC,
                Str = monster.Str,
                Dex = monster.Dex,
                Con = monster.Con,
                Int = monster.Int,
                Wis = monster.Wis,
                Cha = monster.Cha,
                Danger = monster.Danger,
                Description = monster.Description,
                CreatedBy = monster.CreatedBy,
                CreatedByUsername = username,
                Status = monster.Status,
                CreatedAt = monster.CreatedAt,
                UpdatedAt = monster.UpdatedAt
            };

            return CreatedAtAction(nameof(GetMonster), new { id = monster.Id }, result);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<MonsterDto>> UpdateMonster(Guid id, CreateMonsterDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var monster = await _context.Monsters.FindAsync(id);

            if (monster == null)
            {
                return NotFound();
            }

            if (monster.CreatedBy != userId)
            {
                return Forbid();
            }

            monster.Name = updateDto.Name;
            monster.MaxHP = updateDto.MaxHP;
            monster.AC = updateDto.AC;
            monster.Str = updateDto.Str;
            monster.Dex = updateDto.Dex;
            monster.Con = updateDto.Con;
            monster.Int = updateDto.Int;
            monster.Wis = updateDto.Wis;
            monster.Cha = updateDto.Cha;
            monster.Danger = updateDto.Danger;
            monster.Description = updateDto.Description;
            monster.Status = updateDto.Status;
            monster.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var username = await GetUsernameFromGateway(monster.CreatedBy);

            var result = new MonsterDto
            {
                Id = monster.Id,
                Name = monster.Name,
                MaxHP = monster.MaxHP,
                AC = monster.AC,
                Str = monster.Str,
                Dex = monster.Dex,
                Con = monster.Con,
                Int = monster.Int,
                Wis = monster.Wis,
                Cha = monster.Cha,
                Danger = monster.Danger,
                Description = monster.Description,
                CreatedBy = monster.CreatedBy,
                CreatedByUsername = username,
                Status = monster.Status,
                CreatedAt = monster.CreatedAt,
                UpdatedAt = monster.UpdatedAt
            };

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonster(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var monster = await _context.Monsters.FindAsync(id);

            if (monster == null)
            {
                return NotFound();
            }

            if (monster.CreatedBy != userId)
            {
                return Forbid();
            }

            _context.Monsters.Remove(monster);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<string> GetUsernameFromGateway(string userId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri("http://api-gateway-service:8080");

                var cookie = Request.Headers["Cookie"].ToString();
                if (!string.IsNullOrEmpty(cookie))
                {
                    client.DefaultRequestHeaders.Add("Cookie", cookie);
                }

                var response = await client.GetAsync($"/api/auth/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var userData = await response.Content.ReadFromJsonAsync<UserInfo>();
                    return userData?.Username ?? "Unknown";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get username for user {userId}: {ex.Message}");
            }

            return "Unknown";
        }

        private async Task<Dictionary<string, string>> GetUsernamesFromGateway(List<string> userIds)
        {
            var result = new Dictionary<string, string>();

            foreach (var userId in userIds)
            {
                result[userId] = await GetUsernameFromGateway(userId);
            }

            return result;
        }
    }

    public class UserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}