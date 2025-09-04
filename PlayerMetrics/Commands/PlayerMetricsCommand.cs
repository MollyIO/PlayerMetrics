using System;
using System.Linq;
using System.Text;
using CommandSystem;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;

namespace PlayerMetrics.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PlayerMetricsCommand : ParentCommand
    {
        public override string Command { get; } = "playermetrics";
        public override string[] Aliases { get; } = { "plm", "metrics" };
        public override string Description { get; } = "Displays player metrics.";

        public override void LoadGeneratedCommands() {} // Yo, NW remove this shit if you register subs automatically.

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count > 0)
            {
                Player player = Player.Get(sender);
                if (player != null && player.UserGroup == null)
                {
                    arguments[0] = player.UserId;
                }
                
                ICommand command = AllCommands.FirstOrDefault(c => c.Command.Equals("info"));
                if (command != null)
                {
                    return command.Execute(arguments, sender, out response);
                }
            }
            
            StringBuilder sb = new StringBuilder("<color=red>Invalid subcommand. Usage:</color>\n<color=white>---------</color>\n");
            foreach (ICommand command in AllCommands)
            {
                if (!sender.HasPermissions($"{PlayerMetrics.PluginInstance.Name.ToLower()}.{Command.ToLower()}") && PlayerMetrics.PluginInstance.Config != null && PlayerMetrics.PluginInstance.Config.CheckPermissions)
                    continue;
                sb.Append($"{command.Description}\n<color=white>---------</color>\n");
            }
            response = sb.ToString();
            return false;
        }
    }
}