using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheetService.Data;
using SheetService.Models;
using SheetService.DTOs;

namespace SheetService.Controllers
{
    [Route("api/monsters")]
    [ApiController]
    public class MonstersController : ControllerBase
    {
        private readonly SheetDbContext _context;
        private readonly GatewayDbContext _gatewayContext;

        public MonstersController(SheetDbContext context, GatewayDbContext gatewayContext)
        {
            _context = context;
            _gatewayContext = gatewayContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MonsterDto>>> GetMonsters()
        {
            var monsters = await _context.Monsters
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            var creatorIds = monsters.Select(m => m.CreatedBy).Distinct().ToList();
            var usernames = await GetUsernamesFromDb(creatorIds);

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

            var username = await GetUsernameFromDb(monster.CreatedBy);

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
            var username = "";
            Console.WriteLine("========== CREATE MONSTER START ==========");
            Console.WriteLine($"Request received at: {DateTime.UtcNow}");
            Console.WriteLine($"Name: {createDto.Name}");
            Console.WriteLine($"Danger: {createDto.Danger}");
            Console.WriteLine($"Status: {createDto.Status}");

            // ѕроверим, что DTO пришла полностью
            Console.WriteLine($"All fields - MaxHP: {createDto.MaxHP}, AC: {createDto.AC}, Str: {createDto.Str}");

            var userId = "temp-user-id";
            Console.WriteLine($"UserId: {userId}");

            // ѕроверим подключение к GatewayDbContext
            try
            {
                username = await GetUsernameFromDb(userId);
                Console.WriteLine($"Username from DB: {username}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR getting username: {ex.Message}");
                return StatusCode(500, new { error = "Failed to get username" });
            }

            Console.WriteLine("Creating monster object...");



            //var userId = "temp-user-id"; // TODO: получать из авторизации

            var existingMonster = await _context.Monsters
                .FirstOrDefaultAsync(m => m.CreatedBy == userId && m.Name == createDto.Name);

            if (existingMonster != null)
            {
                return Conflict(new
                {
                    error = "” вас уже есть монстр с таким именем",
                    existingMonsterId = existingMonster.Id
                });
            }

            //var username = await GetUsernameFromDb(userId);

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
                CreatedByUsername = username,
                Status = createDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Monsters.Add(monster);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique constraint") == true)
            {
                return Conflict(new { error = "ћонстр с таким именем уже существует" });
            }

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
                CreatedByUsername = monster.CreatedByUsername,
                Status = monster.Status,
                CreatedAt = monster.CreatedAt,
                UpdatedAt = monster.UpdatedAt
            };

            return CreatedAtAction(nameof(GetMonster), new { id = monster.Id }, result);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<MonsterDto>> UpdateMonster(Guid id, CreateMonsterDto updateDto)
        {
            var userId = "temp-user-id"; // TODO: получать из авторизации
            var monster = await _context.Monsters.FindAsync(id);

            if (monster == null)
            {
                return NotFound();
            }

            var existingMonster = await _context.Monsters
                .FirstOrDefaultAsync(m => m.CreatedBy == userId &&
                                           m.Name == updateDto.Name &&
                                           m.Id != id);

            if (existingMonster != null)
            {
                return Conflict(new
                {
                    error = "” вас уже есть другой монстр с таким именем",
                    existingMonsterId = existingMonster.Id
                });
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

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("unique constraint") == true)
            {
                return Conflict(new { error = "ћонстр с таким именем уже существует" });
            }

            var username = await GetUsernameFromDb(monster.CreatedBy);

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
            var monster = await _context.Monsters.FindAsync(id);

            if (monster == null)
            {
                return NotFound();
            }

            _context.Monsters.Remove(monster);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<string> GetUsernameFromDb(string userId)
        {
            var user = await _gatewayContext.Users.FindAsync(userId);
            return user?.Username ?? "Unknown";
        }

        private async Task<Dictionary<string, string>> GetUsernamesFromDb(List<string> userIds)
        {
            if (userIds == null || !userIds.Any())
                return new Dictionary<string, string>();

            return await _gatewayContext.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Username);
        }
    }
}