namespace Natsurainko.FluentLauncher.Services.UI.Navigation;

/// <summary>
/// 提供导航的页面或窗口
/// </summary>
internal interface INavigationProvider
{
    /// <summary>
    /// 提供导航的 UI 元素
    /// </summary>
    object NavigationControl { get; }
}
