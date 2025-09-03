using HarmonyLib;

namespace PlayerMetrics.Patches
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
    internal static class ServerNamePatch
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            ServerConsole.ServerName += $"<color=#00000000><size=1>{PlayerMetrics.PluginInstance.Name} {PlayerMetrics.PluginInstance.Version}</size></color>";
        }
    }
}