using System;
using System.ComponentModel.DataAnnotations;

namespace DNDhelper.Models
{
    public class Session
    {
        [Key]
        public string SessionID { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string UserID { get; set; }

        [Required]
        public string RefreshToken { get; set; }

        public string IpAddress { get; set; }
        public string UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddDays(7);
    }
}
