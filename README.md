<div align="center">

# PlayerMetrics

![Release](https://img.shields.io/github/v/release/MollyIO/PlayerMetrics)
![License](https://img.shields.io/github/license/MollyIO/PlayerMetrics)
![Downloads](https://img.shields.io/github/downloads/MollyIO/PlayerMetrics/total)
![Stars](https://img.shields.io/github/stars/MollyIO/PlayerMetrics)

PlayerMetrics is an SCP:SL server plugin that tracks and displays the time players spend on the server. It is mostly useful for server head admins to track play time requirements for admins and inter admins, but it can also be used in many different ways.
</div>

## Installation
### Automatic Installation
If you are using a Linux server, you can enter this command in your terminal to download and install the plugin automatically:
```bash
curl -s https://raw.githubusercontent.com/MollyIO/PlayerMetrics/main/install.sh | bash
```
### Manual Installation
1. **Download the latest release**<br>
   Go to the [Releases page](https://github.com/MollyIO/PlayerMetrics/releases) and download:
    * `PlayerMetrics.dll`
    * `dependencies.zip`
2. **Locate your LabAPI directories**<br>
   Global paths:
   ```
   <SERVER CONFIG PATH>/LabAPI/plugins/global/
   <SERVER CONFIG PATH>/LabAPI/dependencies/global/
   ```
   If you want to install for a specific server port instead of globally, create folders named with the port number:
   ```
   <SERVER CONFIG PATH>/LabAPI/plugins/<PORT>/
   <SERVER CONFIG PATH>/LabAPI/dependencies/<PORT>/
   ```
3. **Copy files**<br>
    * Place `PlayerMetrics.dll` into the `plugins` folder (`global` or `<PORT>`).
    * Extract `dependencies.zip` into the corresponding `dependencies` folder.
4. **Restart the server**<br>
   After copying the files, restart your SCP: Secret Laboratory server to apply the plugin.
5. **Verify installation**<br>
   Check the server console for messages confirming that PlayerMetrics loaded correctly.

## Configuration
After installation, a configuration file named `config.yaml` will be created in the `<SERVER CONFIG PATH>/LabAPI/config/<PORT OR GLOBAL>/PlayerMetrics` directory. You can modify this file to customize the plugin's behavior. Default config is shown below:
```yaml
# Determines if the plugin should automatically check for updates on startup and download them.
use_auto_updater: true
# Determines if the command checks whether a player has the 'playermetrics.info' or 'playermetrics.top' or 'playermetrics.specific' permission. Disable this to allow all players with access to the RA to use the PlayerMetrics command.
check_permissions: true
```

## Usage
When a player joins the server, their join time is recorded and when they leave, this record is saved to a database. The plugin provides the following commands to access player metrics:

| Command                                                 | Description                                                                                                                                                                                                                                                                | Example                                       |
|---------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-----------------------------------------------|
| `playermetrics info <SteamID or Nickname>`              | Displays detailed metrics for the specified player. If no player is specified, shows metrics for yourself. Metrics include: first seen, last seen, number of sessions, total playtime, average session time, and playtime over the last 1, 3, 7, 14, 30, 90, and 365 days. | `playermetrics info 76561197999281035`        |
| `playermetrics specific <SteamID or Nickname> <period>` | Shows how much time a player has spent on the server during a specific period. Period format: `1y` (year), `31d` (days), `24h` (hours), `60m` (minutes).                                                                                                                   | `playermetrics specific 76561197999281035 7d` |
| `playermetrics top <n>`                                 | Displays the top `n` players with the highest total playtime. Default: `n = 10`, maximum is 150.                                                                                                                                                                           | `playermetrics top 20`                        |

## For developers
If you are a developer and want to contribute to this project, feel free to fork the repository and submit pull requests. 😊<br>
Also, if you are developing a plugin and want to use PlayerMetrics as a dependency, you can include it in your project references and use static Instances in `PlayerMetrics.cs` to access its methods and other stuff.

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
