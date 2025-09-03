using System;

namespace PlayerMetrics.Models
{
    public class ProcessedPlayerData
    {
        public string UserId { get; set; }
        public string Nickname { get; set; }
        public bool Online { get; set; }
        
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public int TotalSessions { get; set; }
        
        public TimeSpan AverageSessionTime { get; set; }
        
        public TimeSpan PlayedLastDay { get; set; }
        public TimeSpan PlayedLast3Days { get; set; }
        public TimeSpan PlayedLast7Days { get; set; }
        public TimeSpan PlayedLast14Days { get; set; }
        public TimeSpan PlayedLast30Days { get; set; }
        public TimeSpan PlayedLast90Days { get; set; }
        public TimeSpan PlayedLast365Days { get; set; }
        public TimeSpan PlayedTotal { get; set; }
    }
}