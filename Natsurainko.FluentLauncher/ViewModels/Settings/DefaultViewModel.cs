﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentLauncher.Infra.Settings.Mvvm;
using FluentLauncher.Infra.UI.Navigation;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Utils;
using Natsurainko.FluentLauncher.ViewModels.Common;
using System.Collections.Generic;
using Windows.ApplicationModel;

#nullable disable
namespace Natsurainko.FluentLauncher.ViewModels.Settings;

internal partial class DefaultViewModel : SettingsViewModelBase, ISettingsViewModel
{
    [SettingsProvider]
    private readonly SettingsService _settingsService;
    private readonly INavigationService _navigationService;

    public List<string> Languages { get; } = ResourceUtils.Languages;

    public DefaultViewModel(SettingsService settingsService, INavigationService navigationService)
    {
        _settingsService = settingsService;
        _navigationService = navigationService;

        (this as ISettingsViewModel).InitializeSettings();
    }

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.CurrentLanguage))]
    private string currentLanguage;

    [ObservableProperty]
    private string version = string.Format("{0}.{1}.{2}.{3}",
        Package.Current.Id.Version.Major,
        Package.Current.Id.Version.Minor,
        Package.Current.Id.Version.Build,
        Package.Current.Id.Version.Revision);

#if DEBUG
    [ObservableProperty]
    private string edition = ResourceUtils.GetValue("Settings", "AboutPage", "_Debug");
#else
    [ObservableProperty]
    private string edition = ResourceUtils.GetValue("Settings", "AboutPage", "_Release");
#endif

    partial void OnCurrentLanguageChanged(string oldValue, string newValue)
    {
        if (Languages.Contains(CurrentLanguage) && oldValue is not null) // oldValue is null at startup
            ResourceUtils.ApplyLanguage(CurrentLanguage);
    }

    [RelayCommand]
    void CardClick(string tag) => _navigationService.NavigateTo(tag);
}
