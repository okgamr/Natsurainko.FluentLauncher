using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using Natsurainko.FluentLauncher.Utils.Extensions;
using System.Collections.ObjectModel;

namespace Natsurainko.FluentLauncher.ViewModels.Settings;

internal partial class SettingsNavigationViewModel : ObservableObject, INavigationAware
{
    private readonly INavigationService _navigationService;

    public ObservableCollection<string> Routes { get; set; } = ["Settings"];

    public SettingsNavigationViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    void INavigationAware.OnNavigatedTo(object? parameter)
        => _navigationService.NavigateTo(parameter is string pageKey ? pageKey : "SettingsCategoryPage");

    [RelayCommand]
    public void ItemClickedEvent(object args)
    {
        var breadcrumbBarItemClickedEventArgs = args.As<BreadcrumbBar, BreadcrumbBarItemClickedEventArgs>().args;

        if (breadcrumbBarItemClickedEventArgs.Item.ToString() == "Settings")
            _navigationService.NavigateTo("SettingsCategoryPage");
    }
}
