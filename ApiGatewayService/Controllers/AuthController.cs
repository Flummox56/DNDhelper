using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ApiGatewayService.Data;
using ApiGatewayService.Models;
using ApiGatewayService.Services;
using BCrypt.Net;

namespace ApiGatewayService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthDbContext _context;
        private readonly SessionService _sessionService;

        public AuthController(AuthDbContext context, SessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest(new { error = "Пользователь с таким именем или email уже существует" });
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Username,
                Email = request.Email,
                Password = passwordHash,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            await LoginUser(user);

            return Ok(new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request) {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null) {
                return Unauthorized(new { error = "Неверное имя пользователя или пароль" });
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password)) {
                return Unauthorized(new { error = "Неверное имя пользователя или пароль" });
            }

            await LoginUser(user);

            return Ok(new { message = "Вход выполнен успешно" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout() {
            var sessionId = HttpContext.Request.Cookies["SessionId"];
            if (!string.IsNullOrEmpty(sessionId)) {
                await _sessionService.TerminateSessionAsync(sessionId);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete("SessionId");

            return Ok(new { message = "Выход выполнен успешно" });
        }

        [HttpGet("profile")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(new { error = "Не авторизован" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { error = "Не авторизован" });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { error = "Пользователь не найден" });
            }

            return Ok(new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserInfo>> GetUserById(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(new UserInfo
            {
                Id = user.Id,
                Username = user.Username
            });
        }

        public class UserInfo
        {
            public string Id { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
        }

        private async Task LoginUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                IssuedUtc = DateTimeOffset.UtcNow
            };

            var session = await _sessionService.CreateSessionAsync(user.Id);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            Response.Cookies.Append("SessionId", session.SessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });
        }
    }

    public class UserResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}