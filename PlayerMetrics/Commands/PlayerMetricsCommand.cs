using System;
using System.Linq;
using System.Text;
using CommandSystem;

namespace PlayerMetrics.Commands
{
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
                ICommand command = AllCommands.FirstOrDefault(c => c.Command.Equals("info"));
                if (command != null)
                {
                    return command.Execute(arguments, sender, out response);
                }
            }
            
            StringBuilder sb = new StringBuilder("<color=red>Invalid subcommand. Usage:</color>\n<color=white>---------</color>\n");
            foreach (ICommand command in AllCommands)
            {
                sb.Append($"{command.Description}\n<color=white>---------</color>\n");
            }
            response = sb.ToString();
            return false;
        }
    }
}