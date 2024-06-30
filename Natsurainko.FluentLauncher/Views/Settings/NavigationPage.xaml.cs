using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using Natsurainko.FluentLauncher.ViewModels.Settings;

namespace Natsurainko.FluentLauncher.Views.Settings;

public sealed partial class NavigationPage : Page, INavigationProvider
{
    object INavigationProvider.NavigationControl => contentFrame;
    private SettingsNavigationViewModel VM => (SettingsNavigationViewModel)DataContext;

    public NavigationPage()
    {
        this.InitializeComponent();
    }

    private void contentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        if (e.SourcePageType == typeof(CategoryPage))
        {
            VM.Routes.Clear();
            VM.Routes.Add("Settings");
        }
        else
        {
            VM.Routes.Add(e.SourcePageType.Name);
        }
    }
}
