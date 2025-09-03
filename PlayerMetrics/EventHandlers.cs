using System;
using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerMetrics.Models;
using PlayerMetrics.Services;

namespace PlayerMetrics
{
    public class EventHandlers
    {
        private static readonly Dictionary<Player, DateTime> LoginTimes = new Dictionary<Player, DateTime>();
        
        public static void OnPlayerJoin(PlayerJoinedEventArgs ev)
        {
            if (ev.Player.DoNotTrack) return;
            
            LoginTimes[ev.Player] = DateTime.Now;
        }
        
        public static void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            if (ev.Player.DoNotTrack) return;
            
            if (!LoginTimes.Remove(ev.Player, out var loginTime))
            {
                Logger.Warn($"Could not find login time for player {ev.Player.Nickname}.");
                return;
            }
            
            PlayerMetrics.DatabaseInstance?.AddPlayerData(new PlayerData
            {
                UserId = ev.Player.UserId,
                Nickname = ev.Player.Nickname,
                LoginTime = loginTime,
                LogoutTime = DateTime.Now
            });
            
            Logger.Info($"Recorded session for player {ev.Player.Nickname} ({ev.Player.UserId}): {DatabaseService.FormatTimeSpan(DateTime.Now - loginTime)}");
        }
    }
}