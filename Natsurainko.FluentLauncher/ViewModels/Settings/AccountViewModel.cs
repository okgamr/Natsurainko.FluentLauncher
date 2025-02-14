﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentLauncher.Infra.Settings.Mvvm;
using Microsoft.Extensions.DependencyInjection;
using Natsurainko.FluentLauncher.Services.Accounts;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Services.Storage;
using Natsurainko.FluentLauncher.Services.UI;
using Natsurainko.FluentLauncher.Utils;
using Natsurainko.FluentLauncher.ViewModels.Common;
using Natsurainko.FluentLauncher.Views;
using Natsurainko.FluentLauncher.Views.Common;
using Nrk.FluentCore.Authentication;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Natsurainko.FluentLauncher.ViewModels.Settings;

internal partial class AccountViewModel : SettingsViewModelBase, ISettingsViewModel
{
    [SettingsProvider]
    private readonly SettingsService _settingsService;
    private readonly AccountService _accountService;
    private readonly AuthenticationService _authenticationService;
    private readonly NotificationService _notificationService;
    private readonly SkinCacheService _skinCacheService;

    public AccountViewModel(
        SettingsService settingsService,
        AccountService accountService,
        AuthenticationService authenticationService,
        NotificationService notificationService,
        SkinCacheService skinCacheService)
    {
        _settingsService = settingsService;
        _accountService = accountService;
        _authenticationService = authenticationService;
        _notificationService = notificationService;
        _skinCacheService = skinCacheService;

        Accounts = accountService.Accounts;
        ActiveAccount = accountService.ActiveAccount;

        (this as ISettingsViewModel).InitializeSettings();
    }

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.EnableDemoUser))]
    private bool enableDemoUser;

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.AutoRefresh))]
    private bool autoRefresh;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SkinFile))]
    [NotifyPropertyChangedFor(nameof(IsOfflineAccount))]
    private Account activeAccount;

    public ReadOnlyObservableCollection<Account> Accounts { get; init; }

    public string SkinFile => _skinCacheService.GetSkinFilePath(ActiveAccount);

    public bool IsOfflineAccount => ActiveAccount.Type == AccountType.Offline;

    partial void OnActiveAccountChanged(Account value)
    {
        if (value is not null) _accountService.ActivateAccount(value);
    }

    [RelayCommand]
    public void Login() => _ = new AuthenticationWizardDialog { XamlRoot = MainWindow.XamlRoot }.ShowAsync();

    [RelayCommand]
    public async Task Refresh()
    {
        var refreshTask = _accountService.RefreshActiveAccount();
        await refreshTask;

        if (refreshTask.IsFaulted)
            _notificationService.NotifyException("_AccountRefreshFailedTitle", refreshTask.Exception, "_AccountRefreshFailedDescription");
        else _notificationService.NotifyMessage(
            ResourceUtils.GetValue("Notifications", "_AccountRefreshedTitle"),
            ResourceUtils.GetValue("Notifications", "_AccountRefreshedDescription").Replace("${name}", _accountService.ActiveAccount.Name));
    }

    [RelayCommand]
    public void Switch()
    {
        var switchAccountDialog = new SwitchAccountDialog
        {
            XamlRoot = Views.MainWindow.XamlRoot,
            DataContext = App.Services.GetService<SwitchAccountDialogViewModel>()
        };
        _ = switchAccountDialog.ShowAsync();
    }

    [RelayCommand]
    public void OpenSkinFile()
    {
        if (!File.Exists(SkinFile))
            return;

        using var process = Process.Start(new ProcessStartInfo("explorer.exe", $"/select,{SkinFile}"));
    }
}