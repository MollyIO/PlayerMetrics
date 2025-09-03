using System;

namespace PlayerMetrics.Models
{
    public class PlayerData
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        
        public DateTime LoginTime { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}