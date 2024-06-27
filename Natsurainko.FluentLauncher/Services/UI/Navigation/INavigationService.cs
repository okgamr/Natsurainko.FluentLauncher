using Microsoft.Extensions.DependencyInjection;

namespace Natsurainko.FluentLauncher.Services.UI.Navigation;

/// <summary>
/// 控制窗口或页面导航的服务
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// 提供导航的窗口或页面
    /// </summary>
    INavigationProvider NavigationProvider { get; }

    /// <summary>
    /// 父窗口或父页面的导航服务（如果支持导航）
    /// </summary>
    INavigationService? Parent { get; }

    /// <summary>
    /// 是否允许返回上一页面
    /// </summary>
    bool CanGoBack { get; }

    /// <summary>
    /// 是否允许进至下一页面
    /// </summary>
    bool CanGoForward { get; }

    /// <summary>
    /// 返回上一页面
    /// </summary>
    void GoBack();

    /// <summary>
    /// 进至下一页面
    /// </summary>
    void GoForward();
    
    /// <summary>
    /// 导航到指定页面
    /// </summary>
    /// <param name="key">页面的注册键名</param>
    /// <param name="parameter">传递的参数</param>
    void NavigateTo(string key, object? parameter = null);

    /// <summary>
    /// 当 NavigationProvider 初始化后会被调用
    /// </summary>
    /// <param name="navigationProvider">提供导航的窗口或页面</param>
    /// <param name="scope">NavigationProvider 的 Scope</param>
    /// 
    // 这是必要的，因为 IServiceScope 不能成为 INavigationService 的依赖项
    // 并且每个 INavigationProvider 都依赖于 INavigationService
    // 因此，只有在创建了 NavigationProvider 及其 Scope 之后才能设置它们
    void InitializeNavigation(INavigationProvider navigationProvider, IServiceScope scope, INavigationService? parent);
}
