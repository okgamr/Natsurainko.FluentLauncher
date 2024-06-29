using Natsurainko.FluentLauncher.Models.Launch;
using Natsurainko.FluentLauncher.Services.Accounts;
using Natsurainko.FluentLauncher.Services.Download;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Services.UI;
using Natsurainko.FluentLauncher.Utils;
using Natsurainko.FluentLauncher.Utils.Extensions;
using Nrk.FluentCore.Authentication;
using Nrk.FluentCore.Environment;
using Nrk.FluentCore.Launch;
using Nrk.FluentCore.Management;
using Nrk.FluentCore.Services.Launch;
using Nrk.FluentCore.Utils;
using PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace Natsurainko.FluentLauncher.Services.Launch;

internal class LaunchService : DefaultLaunchService
{
    private readonly AuthenticationService _authenticationService;
    private readonly DownloadService _downloadService;
    private readonly NotificationService _notificationService;
    private readonly AccountService _accountService;

    private SettingsService AppSettingsService => (SettingsService)_settingsService;

    public LaunchService(
        SettingsService settingsService,
        GameService gameService,
        AccountService accountService,
        AuthenticationService authenticationService,
        DownloadService downloadService,
        NotificationService notificationService)
        : base(settingsService, gameService)
    {
        _authenticationService = authenticationService;
        _accountService = accountService;
        _downloadService = downloadService;
        _notificationService = notificationService;
    }

    public async Task LaunchGame(GameInfo gameInfo)
    {
        try
        {
            Account? account = _accountService.ActiveAccount ?? throw new Exception(LocalizationResourcesUtils.GetValue("Exceptions", "_NoAccount"));

            var session = CreateMinecraftSessionFromGameInfo(gameInfo, account);
            _sessions.Add(session);

            OnSessionCreated(session);

            gameInfo.UpdateLastLaunchTime();
            //App.GetService<JumpListService>().UpdateJumpList(gameInfo);

            await session.StartAsync();
        }
        catch (Exception ex)
        {
            _notificationService.NotifyWithoutContent(ex.Message);
        }
    }

    public override MinecraftSession CreateMinecraftSessionFromGameInfo(GameInfo gameInfo, Account? _)
    {
        Account? account = _accountService.ActiveAccount;
        if (account is null)
            throw new Exception(LocalizationResourcesUtils.GetValue("Exceptions", "_NoAccount"));

        // Java
        string? suitableJava = null;

        if (string.IsNullOrEmpty(_settingsService.ActiveJava))
            throw new Exception(LocalizationResourcesUtils.GetValue("Exceptions", "_NoActiveJava"));
        // TODO: Do not localize exception message

        suitableJava = AppSettingsService.EnableAutoJava ? GetSuitableJava(gameInfo) : _settingsService.ActiveJava;
        if (suitableJava == null)
            throw new Exception(LocalizationResourcesUtils.GetValue("Exceptions", "_NoSuitableJava").Replace("${version}", gameInfo.GetSuitableJavaVersion()));

        var gameConfig = gameInfo.GetConfig(); // Game specific config
        var launchAccount = GetLaunchAccount(gameConfig, _accountService)
            ?? throw new Exception(LocalizationResourcesUtils.GetValue("Exceptions", "_NoAccount")); // Determine which account to use

        Account Authenticate()
        {
            // TODO: refactor to remove dependency on AuthenticationService, and AccountService.
            // Call FluentCore to refresh account directly.
            try
            {
                _accountService.RefreshAccount(launchAccount).GetAwaiter().GetResult();
                return GetLaunchAccount(gameConfig, _accountService);
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        var (maxMemory, minMemory) = AppSettingsService.EnableAutoJava
            ? MemoryUtils.CalculateJavaMemory()
            : (_settingsService.JavaMemory, _settingsService.JavaMemory);

        var session = new MinecraftSession() // Launch session
        {
            Account = launchAccount,
            GameInfo = gameInfo,
            GameDirectory = GetGameDirectory(gameInfo, gameConfig),
            JavaPath = suitableJava,
            MaxMemory = maxMemory,
            MinMemory = minMemory,
            UseDemoUser = _settingsService.EnableDemoUser,
            ExtraGameParameters = GetExtraGameParameters(gameConfig),
            ExtraVmParameters = GetExtraVmParameters(gameConfig, launchAccount),
            CreateResourcesDownloader = (libs) => _downloadService.CreateResourcesDownloader(gameInfo, libs)
        };

        if (AppSettingsService.AutoRefresh)
            session.RefreshAccountTask = new Task<Account>(Authenticate);

        session.ProcessStarted += (s, e) =>
        {
            var title = GameWindowTitle(gameConfig);
            if (string.IsNullOrEmpty(title)) return;

            Task.Run(async () =>
            {
                try
                {
                    while (session.State == MinecraftSessionState.GameRunning)
                    {
                        User32.SetWindowText(session.GetProcessMainWindowHandle(), title);
                        await Task.Delay(1000);
                    }
                }
                catch { }
            });
        };

        return session;
    }

    private string? GetSuitableJava(GameInfo gameInfo)
    {
        var regex = new Regex(@"^([a-zA-Z]:\\)([-\u4e00-\u9fa5\w\s.()~!@#$%^&()\[\]{}+=]+\\?)*$");

        var javaVersion = gameInfo.GetSuitableJavaVersion();
        var suits = new List<(string, Version)>();

        foreach (var java in _settingsService.Javas)
        {
            if (!regex.IsMatch(java) || !File.Exists(java)) continue;

            var info = JavaUtils.GetJavaInfo(java);
            if (info.Version.Major.ToString().Equals(javaVersion))
            {
                suits.Add((java, info.Version));
                suits.Sort((a, b) => -a.Item2.CompareTo(b.Item2));
            }
        }

        if (!suits.Any())
            return null;

        return suits.First().Item1;
    }

    private string GetGameDirectory(GameInfo gameInfo, GameConfig gameConfig)
    {
        if (gameConfig.EnableSpecialSetting)
        {
            if (gameConfig.EnableIndependencyCore)
                return Path.Combine(gameInfo.MinecraftFolderPath, "versions", gameInfo.AbsoluteId);
            else return gameInfo.MinecraftFolderPath;
        }

        if (AppSettingsService.EnableIndependencyCore)
            return Path.Combine(gameInfo.MinecraftFolderPath, "versions", gameInfo.AbsoluteId);

        return gameInfo.MinecraftFolderPath;
    }

    private string? GameWindowTitle(GameConfig gameConfig)
    {
        if (gameConfig.EnableSpecialSetting)
        {
            if (!string.IsNullOrEmpty(gameConfig.GameWindowTitle))
                return gameConfig.GameWindowTitle;
        }
        else
        {
            if (!string.IsNullOrEmpty(AppSettingsService.GameWindowTitle))
                return AppSettingsService.GameWindowTitle;
        }

        return null;
    }

    public static Account GetLaunchAccount(GameConfig gameConfig, AccountService _accountService)
    {
        if (gameConfig.EnableSpecialSetting && gameConfig.EnableTargetedAccount && gameConfig.Account != null)
        {
            var matchAccount = _accountService.Accounts.Where(account =>
            {
                if (!account.Type.Equals(gameConfig.Account.Type)) return false;
                if (!account.Uuid.Equals(gameConfig.Account.Uuid)) return false;
                if (!account.Name.Equals(gameConfig.Account.Name)) return false;

                if (gameConfig.Account is YggdrasilAccount yggdrasil)
                {
                    if (!((YggdrasilAccount)account).YggdrasilServerUrl.Equals(yggdrasil.YggdrasilServerUrl))
                        return false;
                }

                return true;
            });

            if (matchAccount.Any())
                return matchAccount.First();
            else throw new Exception("Can't find target account");
        }

        return _accountService.ActiveAccount;
    }

    private IEnumerable<string> GetExtraVmParameters(GameConfig gameConfig, Account account)
    {
        if (account is YggdrasilAccount yggdrasil)
        {
            using var res = HttpUtils.HttpGet(yggdrasil.YggdrasilServerUrl);

            yield return $"-javaagent:{Path.Combine(Package.Current.InstalledLocation.Path, "Assets", "Libs", "authlib-injector-1.2.5.jar").ToPathParameter()}={yggdrasil.YggdrasilServerUrl}";
            yield return "-Dauthlibinjector.side=client";
            yield return $"-Dauthlibinjector.yggdrasil.prefetched={(res.Content.ReadAsString()).ConvertToBase64()}";
        }

        if (!gameConfig.EnableSpecialSetting || gameConfig.VmParameters == null)
            yield break;

        foreach (var item in gameConfig.VmParameters)
            yield return item;
    }

    private IEnumerable<string> GetExtraGameParameters(GameConfig gameConfig)
    {
        if (gameConfig.EnableSpecialSetting)
        {
            if (gameConfig.EnableFullScreen)
                yield return "--fullscreen";

            if (gameConfig.GameWindowWidth > 0)
                yield return $"--width {gameConfig.GameWindowWidth}";

            if (gameConfig.GameWindowHeight > 0)
                yield return $"--height {gameConfig.GameWindowHeight}";

            if (!string.IsNullOrEmpty(gameConfig.ServerAddress))
            {
                gameConfig.ServerAddress.ParseServerAddress(out var host, out var port);

                yield return $"--server {host}";
                yield return $"--port {port}";
            }
        }
        else
        {
            if (AppSettingsService.EnableFullScreen)
                yield return "--fullscreen";

            if (AppSettingsService.GameWindowWidth > 0)
                yield return $"--width {AppSettingsService.GameWindowWidth}";

            if (AppSettingsService.GameWindowHeight > 0)
                yield return $"--height {AppSettingsService.GameWindowHeight}";

            if (!string.IsNullOrEmpty(AppSettingsService.GameServerAddress))
            {
                gameConfig.ServerAddress.ParseServerAddress(out var host, out var port);

                yield return $"--server {host}";
                yield return $"--port {port}";
            }
        }
    }
}

