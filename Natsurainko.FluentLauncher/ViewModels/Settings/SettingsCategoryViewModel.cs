using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.WinUI.Controls;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using Natsurainko.FluentLauncher.Utils.Extensions;
using Windows.ApplicationModel;

namespace Natsurainko.FluentLauncher.ViewModels.Settings;

internal partial class SettingsCategoryViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public SettingsCategoryViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

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

    [RelayCommand]
    void CardClick(object args)
    {
        var sender = args.As<SettingsCard, object>().sender;
        _navigationService.NavigateTo(sender.GetTag());
    }
}
