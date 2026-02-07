using DNDhelper.Data;
using DNDhelper.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DNDhelper.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<User> Register(string username, string password, string email)
        {
            if (await _db.Users.AnyAsync(u => u.Username == username || u.Email == email))
                throw new Exception("User already exists");

            var hashedPassword = HashPassword(password);

            var user = new User
            {
                Username = username,
                Password = hashedPassword,
                Email = email
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<Session> Login(string username, string password, string ipAddress, string userAgent)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !VerifyPassword(password, user.Password))
                throw new Exception("Invalid credentials");

            var session = new Session
            {
                UserID = user.Id,
                RefreshToken = Guid.NewGuid().ToString(),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            _db.Sessions.Add(session);
            await _db.SaveChangesAsync();
            return session;
        }

        public async Task Logout(string sessionId)
        {
            var session = await _db.Sessions.FirstOrDefaultAsync(s => s.SessionID == sessionId);
            if (session != null)
            {
                _db.Sessions.Remove(session);
                await _db.SaveChangesAsync();
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string hashed)
        {
            return HashPassword(password) == hashed;
        }
    }
}
