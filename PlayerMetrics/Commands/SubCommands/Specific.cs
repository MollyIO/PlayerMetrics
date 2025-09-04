using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using PlayerMetrics.Models;
using PlayerMetrics.Services;

namespace PlayerMetrics.Commands.SubCommands
{
    [CommandHandler(typeof(PlayerMetricsCommand))]
    public class Specific : ICommand
    {
        public string Command { get; } = "specific";
        public string[] Aliases { get; } = { "s" };
        public string Description { get; } = "<color=green>playermetrics specific <b><SteamID or Nickname> <period></b></color> <color=white>- Shows how much time a player has spent in the specified period. The period can be specified in the format: 1y (year), 31d (days), 24h (hours), 60m (minutes).</color>";
        
        public static bool TryParsePeriod(string input, out TimeSpan timeSpan)
        {
            timeSpan = TimeSpan.Zero;
            if (string.IsNullOrEmpty(input))
                return false;

            Match match = Regex.Match(input.Trim(), @"^(\d+)([ydhm])$");
            if (!match.Success)
                return false;

            int value = int.Parse(match.Groups[1].Value);
            switch (match.Groups[2].Value.ToLower())
            {
                case "y":
                    timeSpan = TimeSpan.FromDays(value * 365);
                    break;
                case "d":
                    timeSpan = TimeSpan.FromDays(value);
                    break;
                case "h":
                    timeSpan = TimeSpan.FromHours(value);
                    break;
                case "m":
                    timeSpan = TimeSpan.FromMinutes(value);
                    break;
                default:
                    return false;
            }

            return true;
        }
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.HasPermissions($"{PlayerMetrics.PluginInstance.Name.ToLower()}.{Command.ToLower()}") && PlayerMetrics.PluginInstance.Config != null && PlayerMetrics.PluginInstance.Config.CheckPermissions) // Default variant: playermetrics.specific
            {
                response = "<color=red>You do not have permission to use this command.</color>";
                return false;
            }
            
            if (arguments.Count < 2)
            {
                response = $"<color=red>Usage: {Description}</color>";
                return false;
            }
            
            Player player = Player.Get(sender);
            if (player != null && player.UserGroup == null)
            {
                arguments[0] = player.UserId;
            }
            
            if (!PlayerMetrics.DatabaseInstance.TryGetPlayerId(arguments.At(0), out string userId))
            {
                response = "<color=red>Could not find a player with the specified identifier.</color>";
                return false;
            }

            if (!TryParsePeriod(arguments.At(1), out TimeSpan period))
            {
                response = "<color=red>Invalid period format. Please use the format: 1y (year), 31d (days), 24h (hours), 60m (minutes).</color>";
                return false;
            }

            List<PlayerData> playerDataList = PlayerMetrics.DatabaseInstance.GetPlayerData(userId);
            if (!playerDataList.Any())
            {
                response = "<color=red>No data available for the specified player.</color>";
                return false;
            }
            
            DateTime fromDate = DateTime.Now - period;
            TimeSpan playedTime = playerDataList
                .Where(x => x.LoginTime >= fromDate)
                .Select(x => x.LogoutTime - x.LoginTime)
                .Where(x => x.TotalSeconds > 0)
                .Aggregate(TimeSpan.Zero, (acc, x) => acc + x);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<color=green>Player Metrics for {playerDataList.OrderByDescending(p => p.LoginTime).First().Nickname} ({playerDataList[0].UserId})</color>");
            sb.AppendLine("<color=white>------------------------------------------</color>");
            sb.AppendLine($"<color=yellow>Period:</color> Last {arguments.At(1)} (since {DatabaseService.FormatDateTime(fromDate)})");
            sb.AppendLine($"<color=yellow>Played time:</color> {DatabaseService.FormatTimeSpan(playedTime)}");
            sb.AppendLine("<color=white>------------------------------------------</color>");
            response = sb.ToString();
            return true;
        }
    }
}