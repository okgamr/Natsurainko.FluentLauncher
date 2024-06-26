using AppSettingsManagement;
using AppSettingsManagement.Converters;
using Natsurainko.FluentLauncher.Services.Storage;
using Nrk.FluentCore.Management;
using Nrk.FluentCore.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Windows.Storage;

namespace Natsurainko.FluentLauncher.Services.Settings;

internal partial class SettingsService : SettingsContainer, IFluentCoreSettingsService
{
    #region IFluentCoreSettingsService Property

    public ObservableCollection<string> MinecraftFolders { get; private set; } = new();

    public ObservableCollection<string> Javas { get; private set; } = new();

    [SettingItem(typeof(string), "ActiveMinecraftFolder", Default = "", Converter = typeof(JsonStringConverter<string>))]
    [SettingItem(typeof(GameInfo), "ActiveGameInfo", Converter = typeof(JsonStringConverter<GameInfo>))]
    [SettingItem(typeof(string), "ActiveJava", Default = "", Converter = typeof(JsonStringConverter<string>))]
    [SettingItem(typeof(int), "JavaMemory", Default = 1024, Converter = typeof(JsonStringConverter<int>))]

    #endregion

    #region Fluent Launcher Property

    [SettingItem(typeof(bool), "EnableAutoMemory", Default = true, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(bool), "EnableAutoJava", Default = true, Converter = typeof(JsonStringConverter<bool>))]

    [SettingItem(typeof(bool), "EnableFullScreen", Default = false, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(bool), "EnableIndependencyCore", Default = false, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(string), "GameServerAddress", Default = "", Converter = typeof(JsonStringConverter<string>))]
    [SettingItem(typeof(int), "GameWindowHeight", Default = 480, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(int), "GameWindowWidth", Default = 854, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(string), "GameWindowTitle", Default = "", Converter = typeof(JsonStringConverter<string>))]

    [SettingItem(typeof(bool), "EnableDemoUser", Default = false, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(bool), "AutoRefresh", Default = true, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(Guid), "ActiveAccountUuid")]

    [SettingItem(typeof(string), "CurrentDownloadSource", Default = "Mojang", Converter = typeof(JsonStringConverter<string>))]
    [SettingItem(typeof(bool), "EnableFragmentDownload", Default = true, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(int), "MaxDownloadThreads", Default = 128, Converter = typeof(JsonStringConverter<int>))]

    [SettingItem(typeof(string), "CurrentLanguage", Default = "en-US, English", Converter = typeof(JsonStringConverter<string>))] // TODO: remove default value; set to system language if null
    [SettingItem(typeof(int), "NavigationViewDisplayMode", Default = 0, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(bool), "NavigationViewIsPaneOpen", Default = false, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(int), "DisplayTheme", Default = 0, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(int), "BackgroundMode", Default = 1, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(double), "TintLuminosityOpacity", Default = 0.64, Converter = typeof(JsonStringConverter<double>))]
    [SettingItem(typeof(double), "TintOpacity", Default = 0, Converter = typeof(JsonStringConverter<double>))]
    [SettingItem(typeof(bool), "EnableDefaultAcrylicBrush", Default = false, Converter = typeof(JsonStringConverter<bool>))]
    [SettingItem(typeof(string), "ImageFilePath", Default = "", Converter = typeof(JsonStringConverter<string>))]
    [SettingItem(typeof(int), "SolidSelectedIndex", Default = 0, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(Windows.UI.Color), "SolidCustomColor", Converter = typeof(JsonStringConverter<Windows.UI.Color>))]
    [SettingItem(typeof(Windows.UI.Color), "ThemeCustomColor", Converter = typeof(JsonStringConverter<Windows.UI.Color>))]
    [SettingItem(typeof(bool), "UseSystemAccentColor", Default = true, Converter = typeof(JsonStringConverter<bool>))]

    [SettingItem(typeof(double), "AppWindowHeight", Default = 500, Converter = typeof(JsonStringConverter<double>))]
    [SettingItem(typeof(double), "AppWindowWidth", Default = 950, Converter = typeof(JsonStringConverter<double>))]
    [SettingItem(typeof(WinUIEx.WindowState), "AppWindowState", Default = WinUIEx.WindowState.Normal, Converter = typeof(JsonStringConverter<WinUIEx.WindowState>))]
    [SettingItem(typeof(bool), "FinishGuide", Default = false, Converter = typeof(JsonStringConverter<bool>))]

    [SettingItem(typeof(int), "CoresSortByIndex", Default = 0, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(int), "CoresFilterIndex", Default = 0, Converter = typeof(JsonStringConverter<int>))]
    [SettingItem(typeof(int), "CoresLayoutIndex", Default = 0, Converter = typeof(JsonStringConverter<int>))]

    [SettingItem(typeof(uint), "SettingsVersion", Default = 0u)]

    #endregion

    public SettingsService(ISettingsStorage storage) : base(storage)
    {
        var appsettings = ApplicationData.Current.LocalSettings;

        // Migrate settings data structures from old versions
        Migrate();

        // Init MinecraftFolders
        string[] minecraftFolders = JsonSerializer.Deserialize<string[]>(appsettings.Values["MinecraftFolders"] as string ?? "null")!;
        Array.ForEach(minecraftFolders ?? [], MinecraftFolders.Add);

        MinecraftFolders.CollectionChanged += (sender, e) =>
        {
            appsettings.Values["MinecraftFolders"] = JsonSerializer.Serialize(MinecraftFolders.ToArray());
        };

        // Init Javas
        string[] javaRuntimes = JsonSerializer.Deserialize<string[]>(appsettings.Values["Javas"] as string ?? "null")!;
        Array.ForEach(javaRuntimes ?? [], Javas.Add);

        Javas.CollectionChanged += (sender, e) =>
        {
            appsettings.Values["Javas"] = JsonSerializer.Serialize(Javas.ToArray());
        };
    }

    private void Migrate()
    {
        if (this.CurrentDownloadSource == "Mcbbs")
            this.CurrentDownloadSource = "Bmclapi";

        if (SettingsVersion == 0u) // Version 0: Before Release 2.1.8.0
        {
            MigrateFrom_2_1_8_0();
            SettingsVersion = 1;
        }

        if (SettingsVersion == 1) // Version 1: Before Release 2.1.13.0
        {
            MigrateFrom_2_1_13_0();
            SettingsVersion = 2;
        }
    }

    private static void MigrateFrom_2_1_13_0()
    {
        var appSettings = ApplicationData.Current.LocalSettings;

        if (appSettings.Values["GameFolders"] is string oldGameFolders)
            appSettings.Values["MinecraftFolders"] = oldGameFolders;

        if (appSettings.Values["JavaRuntimes"] is string oldJavaRuntimes)
            appSettings.Values["Javas"] = oldJavaRuntimes;

        if (appSettings.Values["CurrentGameFolder"] is string oldCurrentGameFolder)
            appSettings.Values["ActiveMinecraftFolder"] = oldCurrentGameFolder;

        if (appSettings.Values["CurrentJavaRuntime"] is string oldCurrentJavaRuntime)
            appSettings.Values["ActiveJava"] = oldCurrentJavaRuntime;

        if (appSettings.Values["JavaVirtualMachineMemory"] is string oldJavaVirtualMachineMemory)
            appSettings.Values["JavaMemory"] = oldJavaVirtualMachineMemory;
    }

    private static void MigrateFrom_2_1_8_0()
    {
        var appSettings = ApplicationData.Current.LocalSettings;

        // Migrate the list of accounts from ApplicationData.Current.LocalSettings to LocalFolder/settings/accounts.json
        string accountsJson = appSettings.Values["Accounts"] as string ?? "null";
        JsonNode jsonNode = JsonNode.Parse(accountsJson) ?? new JsonArray();

        string localDataPath = LocalStorageService.LocalFolderPath;
        string accountSettingsDir = Path.Combine(localDataPath, "settings");
        if (!Directory.Exists(accountSettingsDir))
            Directory.CreateDirectory(accountSettingsDir);

        string accountSettingsPath = Path.Combine(accountSettingsDir, "accounts.json");

        File.WriteAllText(accountSettingsPath, jsonNode.ToString());

        // Migrate to storing the GUID of the active account in ApplicationData.Current.LocalSettings

        // Read the old settings entry CurrentAccount in ApplicationData.Current.LocalSettings
        if (appSettings.Values["CurrentAccount"] is not string oldCurrentAccountJson)
            return;
        if (JsonNode.Parse(oldCurrentAccountJson) is not JsonNode currentAccountJsonNode)
            return;

        // Set new setting ActiveAccountUuid and remove the old one
        if (Guid.TryParse(currentAccountJsonNode["Uuid"]!.GetValue<string>(), out Guid currentAccountUuid))
        {
            appSettings.Values["ActiveAccountUuid"] = currentAccountUuid;
        }
    }
}
