using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using LiteDB;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerMetrics.Models;

namespace PlayerMetrics.Services
{
    public class DatabaseService : IDisposable
    {
        private readonly LiteDatabase _database;
        private readonly ILiteCollection<PlayerData> _playerDataCollection;

        public DatabaseService(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Logger.Error("Database path is null or empty. Database service will not be initialized.");
                return;
            }

            _database = new LiteDatabase(path);
            _playerDataCollection = _database.GetCollection<PlayerData>("players");

            _playerDataCollection.EnsureIndex(x => x.UserId);
            _playerDataCollection.EnsureIndex(x => x.Nickname);

            Logger.Info($"Database initialized at '{path}'.");
        }

        public void Dispose()
        {
            _database?.Dispose();
            Logger.Info("Database disposed.");
        }
        
        public bool TryGetPlayerId(string input, out string userId)
        {
            userId = null;
            if (string.IsNullOrEmpty(input))
                return false;

            // Try to get UserId from player if he is online
            if (int.TryParse(input, out int playerId) && Player.TryGet(playerId, out Player playerById))
            {
                userId = playerById.UserId;
                return true;
            }
            
            if (Player.TryGet(input, out Player playerByUserId))
            {
                userId = playerByUserId.UserId;
                return true;
            }
            
            // Try to get UserId from string input (numbers@steam, profile link, raw id, etc.)
            Match match = Regex.Match(input, @"\d{9,}");
            if (match.Success)
            {
                userId = $"{match.Value}@steam";
                return true;
            }

            // Try to get UserId from database by name
            PlayerData playerData = _playerDataCollection.FindOne(x => x.Nickname.Equals(input, StringComparison.OrdinalIgnoreCase));
            if (playerData != null)
            {
                userId = playerData.UserId;
                return true;
            }

            return false;
        }

        public List<PlayerData> GetPlayerData(string input)
        {
            return TryGetPlayerId(input, out var userId)
                ? _playerDataCollection.Find(x => x.UserId == userId).ToList()
                : new List<PlayerData>();
        }
        
        public static string FormatDateTime(DateTime dateTime)
        {
            return dateTime == DateTime.MinValue ? "Never" : dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            timeSpan = timeSpan.Duration();
            if (timeSpan == TimeSpan.Zero)
                return "0 seconds";

            List<string> parts = new List<string>();
            if (timeSpan.Days > 0)
                parts.Add($"{timeSpan.Days} day{(timeSpan.Days > 1 ? "s" : "")}");
            if (timeSpan.Hours > 0)
                parts.Add($"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : "")}");
            if (timeSpan.Minutes > 0)
                parts.Add($"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")}");
            if (timeSpan.Seconds > 0)
                parts.Add($"{timeSpan.Seconds} second{(timeSpan.Seconds > 1 ? "s" : "")}");

            return string.Join(", ", parts);
        }

        
        private static TimeSpan GetPlayedTime(List<PlayerData> playerDataList, DateTime fromDate)
        {
            return playerDataList
                .Where(x => x.LoginTime >= fromDate)
                .Select(x => x.LogoutTime - x.LoginTime)
                .Where(x => x.TotalSeconds > 0)
                .Aggregate(TimeSpan.Zero, (acc, x) => acc + x);
        }

        public ProcessedPlayerData GetProcessedPlayerData(string input)
        {
            List<PlayerData> playerDataList = GetPlayerData(input);
            if (!playerDataList.Any())
                return null;
            
            List<TimeSpan> sessionTimes = playerDataList.Select(x => x.LogoutTime - x.LoginTime).Where(x => x.TotalSeconds > 0).ToList();
            DateTime now = DateTime.Now;
            Player player = Player.Get(playerDataList[0].UserId);

            return new ProcessedPlayerData
            {
                UserId = playerDataList[0].UserId,
                Nickname = player != null ? player.Nickname : playerDataList.OrderByDescending(p => p.LoginTime).First().Nickname,
                Online = player != null,
                
                FirstSeen = playerDataList.Min(x => x.LoginTime),
                LastSeen = playerDataList.Max(x => x.LogoutTime),
                TotalSessions = playerDataList.Count,
                
                AverageSessionTime = sessionTimes.Any() ? TimeSpan.FromTicks((long)sessionTimes.Average(x => x.Ticks)) : TimeSpan.Zero,
                
                PlayedLastDay = GetPlayedTime(playerDataList, now.AddDays(-1)),
                PlayedLast3Days = GetPlayedTime(playerDataList, now.AddDays(-3)),
                PlayedLast7Days = GetPlayedTime(playerDataList, now.AddDays(-7)),
                PlayedLast14Days = GetPlayedTime(playerDataList, now.AddDays(-14)),
                PlayedLast30Days = GetPlayedTime(playerDataList, now.AddDays(-30)),
                PlayedLast90Days = GetPlayedTime(playerDataList, now.AddDays(-90)),
                PlayedLast365Days = GetPlayedTime(playerDataList, now.AddDays(-365)),
                PlayedTotal = sessionTimes.Aggregate(TimeSpan.Zero, (acc, x) => acc + x)
            };
        }
        
        public List<PlayerData> GetAllPlayerData()
        {
            return _playerDataCollection.FindAll().ToList();
        }

        public void AddPlayerData(PlayerData player)
        {
            if (player == null) return;
            _playerDataCollection.Insert(player);
        }
    }
}