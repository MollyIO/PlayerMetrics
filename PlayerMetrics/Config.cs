using System.ComponentModel;

namespace PlayerMetrics
{
    public class Config
    {
        [Description("Determines if the plugin should automatically check for updates on startup and download them.")]
        public bool UseAutoUpdater { get; set; } = true;
        
        [Description("Determines if the command checks whether a player has the 'playermetrics.info' or 'playermetrics.top' or 'playermetrics.specific' permission. Disable this to allow all players with access to the RA to use the PlayerMetrics command.")]
        public bool CheckPermissions { get; set; } = true;
    }
}