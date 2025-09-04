using System;
using System.Linq;
using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using PlayerMetrics.Services;

namespace PlayerMetrics.Commands.SubCommands
{
    [CommandHandler(typeof(PlayerMetricsCommand))]
    public class Top : ICommand
    {
        public string Command { get; } = "top";
        public string[] Aliases { get; } = { "t" };
        public string Description { get; } = "<color=green>playermetrics top <b><n></b></color> <color=white>- Displays the top n players with the highest total playtime. If n is not specified, defaults to 10. Maximum value for n is 150.</color>";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.HasPermissions($"{PlayerMetrics.PluginInstance.Name.ToLower()}.{Command.ToLower()}") && PlayerMetrics.PluginInstance.Config != null && PlayerMetrics.PluginInstance.Config.CheckPermissions) // Default variant: playermetrics.top
            {
                response = "<color=red>You do not have permission to use this command.</color>";
                return false;
            }

            int topCount = 10;
            if (arguments.Count > 0 && !int.TryParse(arguments.At(0), out topCount))
            {
                response = "<color=red>Invalid number format. Please provide a valid integer.</color>";
                return false;
            }

            topCount = Math.Clamp(topCount, 1, 150);

            var allPlayers = PlayerMetrics.DatabaseInstance
                .GetAllPlayerData()
                .GroupBy(p => p.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    g.First().Nickname,
                    TotalPlayed = g.Sum(x => (x.LogoutTime - x.LoginTime).TotalSeconds)
                })
                .OrderByDescending(p => p.TotalPlayed)
                .Take(topCount)
                .ToList();

            if (!allPlayers.Any())
            {
                response = "<color=red>No player data available.</color>";
                return false;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<color=green>Top {topCount} Players by Total Playtime</color>");
            sb.AppendLine("<color=white>------------------------------------------</color>");
            for (int i = 0; i < allPlayers.Count; i++)
            {
                var player = allPlayers[i];
                sb.AppendLine($"<color=yellow>{i + 1}. {player.Nickname} ({player.UserId})</color> - {DatabaseService.FormatTimeSpan(TimeSpan.FromSeconds(player.TotalPlayed))}");
            }
            sb.AppendLine("<color=white>------------------------------------------</color>");
            response = sb.ToString();
            return true;
        }
    }
}