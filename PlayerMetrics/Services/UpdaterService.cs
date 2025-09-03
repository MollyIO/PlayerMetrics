using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;
using PlayerMetrics.Models;

namespace PlayerMetrics.Services
{
    public class UpdaterService
    {
        public static async Task CheckUpdate()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", $"{PlayerMetrics.PluginInstance.Name}-{PlayerMetrics.PluginInstance.Version}");

                    string json = await client.GetStringAsync($"https://api.github.com/repos/{PlayerMetrics.PluginInstance.Author}/{PlayerMetrics.PluginInstance.Name}/releases/latest");
                    GitHubRelease release = JsonSerializer.Deserialize<GitHubRelease>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    Version latestVersion = new Version(release.TagName.TrimStart('v'));

                    if (latestVersion > PlayerMetrics.PluginInstance.Version)
                    {
                        Logger.Info($"New version available: {latestVersion} (Your version: {PlayerMetrics.PluginInstance.Version}). Updating...");

                        await DownloadFile(client, release.GetAssetUrl("PlayerMetrics.dll"), PlayerMetrics.PluginInstance.FilePath);

                        string dependenciesPath = Path.Combine(PathManager.Dependencies.FullName, PlayerMetrics.PluginInstance.FilePath.Contains(Server.Port.ToString()) ? Server.Port.ToString() : "global");
                        await DownloadFile(client, release.GetAssetUrl("dependencies.zip"), Path.Combine(dependenciesPath, "dependencies.zip"));
                        ZipFile.ExtractToDirectory(Path.Combine(dependenciesPath, "dependencies.zip"), dependenciesPath, true);
                        File.Delete(Path.Combine(dependenciesPath, "dependencies.zip"));

                        ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;

                        Logger.Info("Update completed. The server will restart at the end of the round to apply the update.");
                    }
                    else
                    {
                        Logger.Info("Your version is up to date.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Updater error: {ex}");
            }
        }

        private static async Task DownloadFile(HttpClient client, string url, string path)
        {
            using (var response = await client.GetAsync(url))
            {
                response.EnsureSuccessStatusCode();

                using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }
        }
    }
}