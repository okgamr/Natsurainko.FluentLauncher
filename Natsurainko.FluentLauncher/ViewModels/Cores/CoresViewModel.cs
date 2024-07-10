using AppSettingsManagement.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Natsurainko.FluentLauncher.Services.Launch;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Services.UI;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using Nrk.FluentCore.Management;
using System.Collections.ObjectModel;
using System.ComponentModel;

#nullable disable

namespace Natsurainko.FluentLauncher.ViewModels.Cores;

internal partial class CoresViewModel : ObservableObject, ISettingsViewModel
{
    private bool initSettings = false;

    [SettingsProvider]
    private readonly SettingsService _settingsService;
    private readonly INavigationService _navigationService;
    private readonly GameService _gameService;
    private readonly NotificationService _notificationService;

    public CoresViewModel(
        GameService gameService,
        SettingsService settingsService,
        INavigationService navigationService,
        NotificationService notificationService)
    {
        _gameService = gameService;
        _settingsService = settingsService;
        _navigationService = navigationService;
        _notificationService = notificationService;

        GameInfos = _gameService.Games;

        (this as ISettingsViewModel).InitializeSettings();
        initSettings = true;
    }

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.CoresLayoutIndex))]
    private int segmentedSelectedIndex;

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.CoresFilterIndex))]
    private int filterIndex;

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.CoresSortByIndex))]
    private int sortByIndex;

    [ObservableProperty]
    [BindToSetting(Path = nameof(SettingsService.ActiveMinecraftFolder))]
    private string activeMinecraftFolder;

    public ReadOnlyObservableCollection<GameInfo> GameInfos { get; init; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (!initSettings)
            return;
    }

    [RelayCommand]
    public void GoToSettings() => _navigationService.NavigateTo("SettingsNavigationPage", "LaunchSettingsPage");
}
