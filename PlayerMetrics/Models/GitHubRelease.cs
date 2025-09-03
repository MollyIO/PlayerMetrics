using System;

namespace PlayerMetrics.Models
{
    public class GitHubRelease
    {
        public string TagName { get; set; }
        public GitHubAsset[] Assets { get; set; }

        public string GetAssetUrl(string name)
        {
            foreach (GitHubAsset asset in Assets)
            {
                if (asset.Name == name)
                {
                    return asset.BrowserDownloadUrl;
                }
            }
            
            throw new Exception($"Asset {name} not found in release.");
        }
    }
}