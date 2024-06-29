using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Natsurainko.FluentLauncher.Services.Settings;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using Natsurainko.FluentLauncher.Services.UI.Pages;
using Natsurainko.FluentLauncher.Utils.Extensions;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.Graphics;

namespace Natsurainko.FluentLauncher.Views;

public sealed partial class ShellPage : Page, INavigationProvider
{
    object INavigationProvider.NavigationControl => contentFrame;
    //private ShellViewModel VM => (ShellViewModel)DataContext;*/

    public static XamlRoot _XamlRoot { get; private set; } = null!; // Initialized on Page_Loaded
    public static Frame ContentFrame { get; private set; } = null!; // Initialized on Page_Loaded

    private readonly SettingsService _settings = App.GetService<SettingsService>();
    //private readonly AppearanceService _appearanceService = App.GetService<AppearanceService>();

    public ShellPage()
    {
        this.InitializeComponent();

        ContentFrame = contentFrame;
    }

    #region Page Events

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        _XamlRoot = XamlRoot;
        App.MainWindow.SetTitleBar(AppTitleBar);

        RefreshDragArea();
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        AppTitle.Visibility = e.NewSize.Width <= 750 ? Visibility.Collapsed : Visibility.Visible;

        RefreshDragArea();
    }

    #endregion

    #region NavigationView Events
    private void NavigationViewControl_PaneClosing(NavigationView sender, object _)
    {
        AutoSuggestBox.Visibility = Visibility.Visible;

        UpdateAppTitleMargin(sender);
        RefreshDragArea();
    }

    private void NavigationViewControl_PaneOpening(NavigationView sender, object _)
    {
        AutoSuggestBox.Visibility = NavigationViewControl.DisplayMode == NavigationViewDisplayMode.Minimal ? Visibility.Collapsed : Visibility.Visible;
        
        UpdateAppTitleMargin(sender);
        RefreshDragArea();
    }

    private void NavigationViewControl_ItemInvoked(NavigationView _, NavigationViewItemInvokedEventArgs args)
    {
        var pageTag = ((NavigationViewItem)args.InvokedItemContainer).GetTag();

        //VM.NavigationService.NavigateTo(pageTag);
    }

    private void NavigationViewControl_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) { } // => VM.NavigationService.GoBack();

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        PaneToggleButton.Visibility = args.DisplayMode == NavigationViewDisplayMode.Minimal ? Visibility.Visible : Visibility.Collapsed;
        NavigationViewControl.IsPaneToggleButtonVisible = args.DisplayMode != NavigationViewDisplayMode.Minimal;

        if (args.DisplayMode == NavigationViewDisplayMode.Minimal)
        {
            Grid.SetRow(NavigationViewControl, 0);
            Grid.SetRowSpan(NavigationViewControl, 2);
            Spacer.Height = 48;
            contentFrame.Margin = new Thickness(0, 48, 0, 0);
        }
        else
        {
            Grid.SetRow(NavigationViewControl, 1);
            Grid.SetRowSpan(NavigationViewControl, 1);
            Spacer.Height = 0;
            contentFrame.Margin = new Thickness(0);
        }

        UpdateAppTitleMargin(sender);
        RefreshDragArea();
    }

    #endregion

    private void RefreshDragArea()
    {
        var scaleAdjustment = XamlRoot.RasterizationScale;
        var height = (int)(48 * scaleAdjustment);

        if (AutoSuggestBox.Visibility == Visibility.Collapsed)
        {
            App.MainWindow.AppWindow.TitleBar.SetDragRectangles([ new() 
            {
                X = (int)(Column0.ActualWidth * scaleAdjustment),
                Y = 0,
                Width = (int)((this.ActualWidth - Column0.ActualWidth) * scaleAdjustment),
                Height = height
            }]);

            return;
        }

        var transform = AutoSuggestBox.TransformToVisual(AppTitleBar);
        var absolutePosition = transform.TransformPoint(new Point(0, 0));

        var dragRects = new List<RectInt32>
        {
            new() 
            {
                X = (int)(Column0.ActualWidth * scaleAdjustment),
                Y = 0,
                Width = (int)((absolutePosition.X - Column0.ActualWidth) * scaleAdjustment),
                Height = height
            },
            new()
            {
                X = (int)((absolutePosition.X + AutoSuggestBox.ActualWidth) * scaleAdjustment),
                Y = 0,
                Width = (int)((AppTitleBar.ActualWidth - (absolutePosition.X + AutoSuggestBox.ActualWidth)) * scaleAdjustment),
                Height = height
            }
        };

        App.MainWindow.AppWindow.TitleBar.SetDragRectangles([.. dragRects]);
    }

    private void UpdateAppTitleMargin(NavigationView sender)
    {
        AppTitle.TranslationTransition = new Vector3Transition();
        AppTitle.Translation = ((sender.DisplayMode == NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                 sender.DisplayMode == NavigationViewDisplayMode.Minimal)
                 ? new System.Numerics.Vector3(8, 0, 0)
                 : new System.Numerics.Vector3(28, 0, 0);
    }

    private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
    {
        foreach (NavigationViewItem item in NavigationViewControl.MenuItems.Union(NavigationViewControl.FooterMenuItems).Cast<NavigationViewItem>())
        {
            string tag = item.GetTag();

            if (App.GetService<IPageProvider>().RegisteredPages[tag].PageType == e.SourcePageType)
            {
                NavigationViewControl.SelectedItem = item;
                item.IsSelected = true;
                return;
            }
        }
    }

    private void PaneToggleButton_Click(object sender, RoutedEventArgs e)
    {
        NavigationViewControl.IsPaneOpen = !NavigationViewControl.IsPaneOpen;
    }
}
