using Natsurainko.FluentLauncher.Models.Launch;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Services.Storage;
using Nrk.FluentCore.Authentication;
using Nrk.FluentCore.Management;
using Nrk.FluentCore.Management.ModLoaders;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Natsurainko.FluentLauncher.Utils.Extensions;

internal static class GameInfoExtensions
{
    public static GameConfig GetConfig(this GameInfo gameInfo)
    {
        var localStorageService = App.GetService<LocalStorageService>();

        var configGuid = new Guid(MD5.HashData(Encoding.UTF8.GetBytes($"{gameInfo.MinecraftFolderPath}:{gameInfo.AbsoluteId}:{gameInfo.Type}")));
        var configStorageFolder = localStorageService.GetDirectory("GameConfigurations");
        var configFile = localStorageService.GetFile($"GameConfigurations\\{configGuid}.json");

        if (!configFile.Exists) 
            return new GameConfig { FilePath = configFile.FullName };

        JsonObject json = JsonNode.Parse(File.ReadAllText(configFile.FullName))!.AsObject();

        var accountNode = json["Account"];

        if (accountNode != null)
            json.Remove("Account");

        GameConfig gameConfig = json.Deserialize<GameConfig>()!;

        if (accountNode != null)
        {
            var accountType = (AccountType)(accountNode?["Type"]!.GetValue<int>())!;

            gameConfig.Account = accountType switch
            {
                AccountType.Offline => accountNode.Deserialize<OfflineAccount>()!,
                AccountType.Microsoft => accountNode.Deserialize<MicrosoftAccount>()!,
                AccountType.Yggdrasil => accountNode.Deserialize<YggdrasilAccount>()!,
                _ => throw new ArgumentException("E008")
            };
        }

        gameConfig.FilePath = configFile.FullName;

        return gameConfig;
    }

    public static bool IsSupportMod(this GameInfo gameInfo)
    {
        if (gameInfo.IsVanilla) return false;

        var loaders = gameInfo.GetModLoaders().Select(x => x.LoaderType).ToArray();

        if (!(loaders.Contains(ModLoaderType.Forge) ||
            loaders.Contains(ModLoaderType.Fabric) ||
            loaders.Contains(ModLoaderType.NeoForge) ||
            loaders.Contains(ModLoaderType.Quilt) ||
            loaders.Contains(ModLoaderType.LiteLoader)))
            return false;

        return true;
    }

    public static string GetGameDirectory(this GameInfo gameInfo)
    {
        var specialConfig = gameInfo.GetConfig();

        if (specialConfig.EnableSpecialSetting)
        {
            if (specialConfig.EnableIndependencyCore)
                return Path.Combine(gameInfo.MinecraftFolderPath, "versions", gameInfo.AbsoluteId);
            else return gameInfo.MinecraftFolderPath;
        }

        if (App.GetService<SettingsService>().EnableIndependencyCore)
            return Path.Combine(gameInfo.MinecraftFolderPath, "versions", gameInfo.AbsoluteId);

        return gameInfo.MinecraftFolderPath;
    }

    public static void UpdateLastLaunchTime(this GameInfo gameInfo)
    {
        var specialConfig = gameInfo.GetConfig();

        // Update launch time
        var launchTime = DateTime.Now;
        specialConfig.LastLaunchTime = launchTime;
    }
}
