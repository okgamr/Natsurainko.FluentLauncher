using AppSettingsManagement.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using Natsurainko.FluentLauncher.Utils;
using System.Collections.Generic;
using Windows.ApplicationModel;

#nullable disable

namespace Natsurainko.FluentLauncher.ViewModels.Settings;

internal partial class SettingsCategoryViewModel : SettingsViewModelBase, ISettingsViewModel
{
    [SettingsProvider]
    private readonly SettingsService _settingsService;

    private readonly INavigationService _navigationService;

    public SettingsCategoryViewModel(SettingsService settingsService, INavigationService navigationService)
    {
        _settingsService = settingsService;
        _navigationService = navigationService;

        (this as ISettingsViewModel).InitializeSettings();
    }

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.CurrentLanguage))]
    private string currentLanguage;

    public List<string> Languages { get; } = LocalizationResourcesUtils.Languages;

    [ObservableProperty]
    private string version = string.Format("{0}.{1}.{2}.{3}",
        Package.Current.Id.Version.Major,
        Package.Current.Id.Version.Minor,
        Package.Current.Id.Version.Build,
        Package.Current.Id.Version.Revision);

#if DEBUG
    [ObservableProperty]
    private string edition = "Debug";
#else
    [ObservableProperty]
    private string edition = "Release";
#endif

    partial void OnCurrentLanguageChanged(string oldValue, string newValue)
    {
        if (Languages.Contains(CurrentLanguage) && oldValue is not null) // oldValue is null at startup
            LocalizationResourcesUtils.ApplyLanguage(CurrentLanguage);
    }

    [RelayCommand]
    void CardClick(string tag) => _navigationService.NavigateTo(tag);
}
