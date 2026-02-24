using Microsoft.EntityFrameworkCore;
using DNDhelper.Data;
using DNDhelper.Models;
using System.Security.Cryptography;

namespace DNDhelper.Services
{
    public class SessionService
    {
        private readonly AuthDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(AuthDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Session> CreateSessionAsync(string userId)
        {
            var sessionId = GenerateSecureSessionId();

            var session = new Session
            {
                SessionId = sessionId,
                UserId = userId,
                RefreshToken = GenerateRefreshToken(),
                IpAddress = GetClientIpAddress(),
                UserAgent = GetUserAgent(),
                CreatedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(7)
            };

            _context.Sessions.Add(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<Session?> GetValidSessionAsync(string sessionId)
        {
            return await _context.Sessions
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId &&
                                         s.ExpiredAt > DateTime.UtcNow);
        }

        public async Task<bool> ExtendSessionAsync(string sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null && session.ExpiredAt > DateTime.UtcNow)
            {
                session.ExpiredAt = DateTime.UtcNow.AddDays(7);
                session.CreatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task TerminateSessionAsync(string sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);
            if (session != null)
            {
                session.ExpiredAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task TerminateAllUserSessionsAsync(string userId, string? currentSessionId = null)
        {
            var sessions = await _context.Sessions
                .Where(s => s.UserId == userId && s.ExpiredAt > DateTime.UtcNow)
                .ToListAsync();

            foreach (var session in sessions)
            {
                if (session.SessionId != currentSessionId)
                {
                    session.ExpiredAt = DateTime.UtcNow;
                }
            }
            await _context.SaveChangesAsync();
        }

        private string GenerateSecureSessionId()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes)
                    .Replace("/", "_")
                    .Replace("+", "-")
                    .Replace("=", "");
            }
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }

        private string GetClientIpAddress()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context != null)
            {
                var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',').First().Trim();
                }

                return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            }
            return "unknown";
        }

        private string GetUserAgent()
        {
            return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "unknown";
        }
    }
}