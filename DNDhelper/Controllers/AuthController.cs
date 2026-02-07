using Microsoft.AspNetCore.Mvc;
using DNDhelper.Services;
using DNDhelper.Data;
using DNDhelper.Models;

namespace DNDhelper.Controllers
{
    [ApiController]
    [Route("/")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;
        private readonly AppDbContext _db;

        public AuthController(AuthService auth, AppDbContext db)
        {
            _auth = auth;
            _db = db;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User request)
        {
            try
            {
                var user = await _auth.Register(request.Username, request.Password, request.Email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User request)
        {
            try
            {
                var ip = HttpContext.Connection.RemoteIpAddress?.ToString();
                var ua = Request.Headers["User-Agent"].ToString();

                var session = await _auth.Login(request.Username, request.Password, ip, ua);

                Response.Cookies.Append("sessionId", session.SessionID, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = session.ExpiredAt
                });

                return Ok(new
                {
                    sessionId = session.SessionID,
                    refreshToken = session.RefreshToken
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var sessionId = Request.Cookies["sessionId"];
            if (!string.IsNullOrEmpty(sessionId))
            {
                await _auth.Logout(sessionId);
                Response.Cookies.Delete("sessionId");
            }
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            var sessionId = Request.Cookies["sessionId"];
            if (string.IsNullOrEmpty(sessionId))
                return Unauthorized(new { message = "No session" });

            var session = await _db.Sessions.FindAsync(sessionId);
            if (session == null || session.ExpiredAt < DateTime.UtcNow)
                return Unauthorized(new { message = "Invalid or expired session" });

            var user = await _db.Users.FindAsync(session.UserID);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt,
                user.UpdatedAt
            });
        }
    }
}
