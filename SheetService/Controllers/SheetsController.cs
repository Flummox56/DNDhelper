using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using SheetService.Data;
using SheetService.Models;

namespace SheetService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SheetsController : ControllerBase
    {
        private readonly SheetDbContext _context;

        public SheetsController(SheetDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CharacterSheet>>> GetMySheets()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var sheets = await _context.CharacterSheets
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.UpdatedAt)
                .ToListAsync();

            return Ok(sheets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CharacterSheet>> GetSheet(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sheet = await _context.CharacterSheets.FindAsync(id);

            if (sheet == null)
                return NotFound();

            if (sheet.UserId != userId)
                return Forbid();

            return Ok(sheet);
        }

        [HttpPost]
        public async Task<ActionResult<CharacterSheet>> CreateSheet(CharacterSheet sheet)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            sheet.UserId = userId;
            sheet.Id = Guid.NewGuid();
            sheet.CreatedAt = DateTime.UtcNow;
            sheet.UpdatedAt = DateTime.UtcNow;

            _context.CharacterSheets.Add(sheet);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSheet), new { id = sheet.Id }, sheet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSheet(Guid id, CharacterSheet sheet)
        {
            if (id != sheet.Id)
                return BadRequest();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var existingSheet = await _context.CharacterSheets.FindAsync(id);

            if (existingSheet == null)
                return NotFound();

            if (existingSheet.UserId != userId)
                return Forbid();

            existingSheet.CharacterName = sheet.CharacterName;
            existingSheet.Race = sheet.Race;
            existingSheet.Class = sheet.Class;
            existingSheet.Level = sheet.Level;
            existingSheet.Strength = sheet.Strength;
            existingSheet.Dexterity = sheet.Dexterity;
            existingSheet.Constitution = sheet.Constitution;
            existingSheet.Intelligence = sheet.Intelligence;
            existingSheet.Wisdom = sheet.Wisdom;
            existingSheet.Charisma = sheet.Charisma;
            existingSheet.Skills = sheet.Skills;
            existingSheet.Inventory = sheet.Inventory;
            existingSheet.Spells = sheet.Spells;
            existingSheet.Notes = sheet.Notes;
            existingSheet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSheet(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sheet = await _context.CharacterSheets.FindAsync(id);

            if (sheet == null)
                return NotFound();

            if (sheet.UserId != userId)
                return Forbid();

            _context.CharacterSheets.Remove(sheet);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}