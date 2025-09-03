using System;
using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using PlayerMetrics.Models;
using PlayerMetrics.Services;

namespace PlayerMetrics.Commands.SubCommands
{
    [CommandHandler(typeof(PlayerMetricsCommand))]
    public class Info : ICommand
    {
        public string Command { get; } = "info";
        public string[] Aliases { get; } = { "i" };
        public string Description { get; } = "<color=green>playermetrics info <b><SteamID or Nickname></b></color> <color=white>- Displays detailed metrics for the specified player. if no player is specified, shows info for the command sender.</color>";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.HasPermissions($"{PlayerMetrics.PluginInstance.Name.ToLower()}.{Command.ToLower()}")) // Default variant: playermetrics.info
            {
                response = "<color=red>You do not have permission to use this command.</color>";
                return false;
            }
            
            if (arguments.Count < 1)
            {
                arguments[0] = Player.Get(sender)?.UserId;
            }
            
            ProcessedPlayerData processedPlayerData = PlayerMetrics.DatabaseInstance.GetProcessedPlayerData(arguments.At(0));
            if (processedPlayerData == null)
            {
                response = "<color=red>No data available for the specified player.</color>";
                return false;
            }
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<color=green>Player Metrics for {processedPlayerData.Nickname} ({processedPlayerData.UserId}) ({(processedPlayerData.Online ? "Online" : "Offline")})</color>");
            sb.AppendLine("<color=white>------------------------------------------</color>");
            sb.AppendLine($"<color=yellow>First Seen:</color> {DatabaseService.FormatDateTime(processedPlayerData.FirstSeen)} ({DatabaseService.FormatTimeSpan(DateTime.Now - processedPlayerData.FirstSeen)} ago)");
            sb.AppendLine($"<color=yellow>Last Seen:</color> {DatabaseService.FormatDateTime(processedPlayerData.LastSeen)} ({DatabaseService.FormatTimeSpan(DateTime.Now - processedPlayerData.LastSeen)} ago)");
            sb.AppendLine($"<color=yellow>Number of Sessions:</color> {processedPlayerData.TotalSessions}");
            sb.AppendLine("<color=white>------------------------------------------</color>");
            sb.AppendLine($"<color=yellow>Played last day:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedLastDay)}");
            sb.AppendLine($"<color=yellow>Played last 3 days:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedLast3Days)}");
            sb.AppendLine($"<color=yellow>Played last 7 days:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedLast7Days)}");
            sb.AppendLine($"<color=yellow>Played last 14 days:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedLast14Days)}");
            sb.AppendLine($"<color=yellow>Played last 30 days:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedLast30Days)}");
            sb.AppendLine($"<color=yellow>Played last 90 days:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedLast90Days)}");
            sb.AppendLine($"<color=yellow>Played last 365 days:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedLast365Days)}");
            sb.AppendLine($"<color=yellow>Played total:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.PlayedTotal)}");
            sb.AppendLine($"<color=yellow>Average Session Time:</color> {DatabaseService.FormatTimeSpan(processedPlayerData.AverageSessionTime)}");
            sb.AppendLine("<color=white>------------------------------------------</color>");
            response = sb.ToString();
            return true;
        }
    }
}