namespace Natsurainko.FluentLauncher.Services.UI.Navigation;

/// <summary>
/// 由可以响应导航事件的 ViewModel 实现
/// </summary>
internal interface INavigationAware
{
    void OnNavigatedTo(object? parameter) { }

    void OnNavigatedFrom() { }
}
