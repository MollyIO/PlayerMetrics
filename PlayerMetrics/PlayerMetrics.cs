using System;
using System.IO;
using HarmonyLib;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using LabApi.Loader.Features.Plugins;
using PlayerMetrics.Services;

namespace PlayerMetrics
{
    public class PlayerMetrics : Plugin<Config>
    {
        public override string Name { get; } = "PlayerMetrics";
        public override string Description { get; } = "A plugin to track and display player metrics.";
        public override string Author { get; } = "MollyIO";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        
        public static PlayerMetrics PluginInstance { get; private set; }
        public static DatabaseService DatabaseInstance { get; private set; }
        public static Harmony HarmonyInstance { get; private set; }
        
        public override void Enable()
        {
            PluginInstance = this;
            DatabaseInstance = new DatabaseService(Path.Combine(PathManager.Configs.FullName, Server.Port.ToString(), "PlayerMetrics.db"));
            HarmonyInstance = new Harmony($"{Author.ToLower()}.{Name.ToLower()}");
            
            HarmonyInstance.PatchAll();
            
            PlayerEvents.Joined += EventHandlers.OnPlayerJoin;
            PlayerEvents.Left += EventHandlers.OnPlayerLeft;
            
            _ = UpdaterService.CheckUpdate();
        }

        public override void Disable()
        {
            PlayerEvents.Joined -= EventHandlers.OnPlayerJoin;
            PlayerEvents.Left -= EventHandlers.OnPlayerLeft;
            
            HarmonyInstance?.UnpatchAll(HarmonyInstance.Id);
            DatabaseInstance?.Dispose();
            
            HarmonyInstance = null;
            DatabaseInstance = null;
            PluginInstance = null;
        }
    }
}
