using System;

namespace DNDhelper.Models
{
    public class Session
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddDays(7);

        public User User { get; set; } = null!;
    }
}