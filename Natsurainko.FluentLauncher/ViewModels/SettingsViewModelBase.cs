using AppSettingsManagement.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Natsurainko.FluentLauncher.ViewModels;

abstract class SettingsViewModelBase : ObservableObject
{
    ~SettingsViewModelBase()
    {
        if (this is ISettingsViewModel settingsViewModel)
            settingsViewModel.RemoveSettingsChagnedHandlers();
    }
}
