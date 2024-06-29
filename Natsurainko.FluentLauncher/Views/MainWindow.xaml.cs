using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Services.UI;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using Natsurainko.FluentLauncher.Utils;
using System.IO;
using Windows.ApplicationModel;
using Windows.Globalization;
using WinUIEx;

namespace Natsurainko.FluentLauncher.Views;

public sealed partial class MainWindow : WindowEx, INavigationProvider
{
    public Frame ContentFrame => Frame;

    object INavigationProvider.NavigationControl => Frame;

    private readonly INavigationService _navService;
    private readonly SettingsService _settings;
    private readonly NotificationService _notificationService = App.GetService<NotificationService>();
    private bool _firstActivated = true;

    public MainWindow(SettingsService settingsService, INavigationService navService)
    {
        this.InitializeComponent();

        _settings = settingsService;
        _navService = navService;

        ConfigureWindow();
    }

    void ConfigureWindow()
    {
        App.MainWindow = this;
        var hoverColor = App.Current.RequestedTheme == ApplicationTheme.Light ? Colors.Black : Colors.White;
        hoverColor.A = 35;

        if (string.IsNullOrEmpty(ApplicationLanguages.PrimaryLanguageOverride))
            LocalizationResourcesUtils.ApplyLanguage(_settings.CurrentLanguage);

        InitializeComponent();

        _notificationService.InitContainer(NotifyStackPanel, BackgroundGrid);

        AppWindow.SetIcon(Path.Combine(Package.Current.InstalledLocation.Path, "Assets/AppIcon.ico"));
        AppWindow.Title = "Fluent Launcher";
        AppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
        AppWindow.TitleBar.ButtonBackgroundColor = AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonForegroundColor = App.Current.RequestedTheme == ApplicationTheme.Light ? Colors.Black : Colors.White;
        AppWindow.TitleBar.ButtonHoverForegroundColor = App.Current.RequestedTheme == ApplicationTheme.Light ? Colors.Black : Colors.White;
        AppWindow.TitleBar.ButtonHoverBackgroundColor = hoverColor;

        (MinWidth, MinHeight) = _settings.FinishGuide ? (516, 328) : (_settings.AppWindowWidth, _settings.AppWindowHeight);
        (Width, Height) = (_settings.AppWindowWidth, _settings.AppWindowHeight);

        //App.GetService<AppearanceService>().ApplyBackgroundAtWindowCreated(this);
        this.SystemBackdrop = new MicaBackdrop();

        this.SizeChanged += WindowEx_SizeChanged;
        this.WindowStateChanged += WindowEx_WindowStateChanged;
        this.Activated += WindowEx_Activated;
    }

    #region Window Events

    private void WindowEx_WindowStateChanged(object? sender, WindowState e)
    {
        _settings.AppWindowState = e;
    }

    private void WindowEx_SizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        _settings.AppWindowWidth = App.MainWindow.Width;
        _settings.AppWindowHeight = App.MainWindow.Height;
    }

    private void WindowEx_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (_firstActivated)
        {
            _navService.NavigateTo(_settings.FinishGuide ? "ShellPage" : "OOBENavigationPage");
            this.CenterOnScreen();
            if (_settings.AppWindowState == WindowState.Maximized) this.Maximize();
        }

        _firstActivated = false;
    }

    private void Grid_ActualThemeChanged(FrameworkElement sender, object args)
    {
        AppWindow.TitleBar.ButtonBackgroundColor = AppWindow.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        AppWindow.TitleBar.ButtonForegroundColor = App.Current.RequestedTheme == ApplicationTheme.Light ? Colors.Black : Colors.White;
        AppWindow.TitleBar.ButtonHoverForegroundColor = App.Current.RequestedTheme == ApplicationTheme.Light ? Colors.Black : Colors.White;

        var hoverColor = App.Current.RequestedTheme == ApplicationTheme.Light ? Colors.Black : Colors.White;
        hoverColor.A = 35;

        AppWindow.TitleBar.ButtonHoverBackgroundColor = hoverColor;
    }

    #endregion
}
