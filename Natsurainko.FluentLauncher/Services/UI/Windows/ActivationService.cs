using Microsoft.Extensions.DependencyInjection;
using Natsurainko.FluentLauncher.Services.UI.Navigation;
using System;
using System.Collections.Generic;

namespace Natsurainko.FluentLauncher.Services.UI.Windows;

/// <summary>
/// <see cref="IActivationService"/> 的默认实现
/// </summary>
internal abstract class ActivationService<TWindowBase> : IActivationService
{
    protected readonly IServiceProvider _windowProvider;
    protected readonly IReadOnlyDictionary<string, WindowDescriptor> _registeredWindows;
    protected readonly List<TWindowBase> _activeWindows = new();

    public IReadOnlyDictionary<string, WindowDescriptor> RegisteredWindows => _registeredWindows;

    /// <summary>
    /// 创建支持激活所述窗口的激活服务
    /// </summary>
    /// <param name="registeredWindows">将字符串键映射到 <see cref="WindowDescriptor"/> 对象的只读字典.</param>
    /// <param name="windowProvider">An <see cref="IServiceProvider"/> that has been configured to support window types according to the rules defined by <paramref name="registeredWindows"/>.</param>
    internal ActivationService(IReadOnlyDictionary<string, WindowDescriptor> registeredWindows, IServiceProvider windowProvider)
    {
        _registeredWindows = registeredWindows;
        _windowProvider = windowProvider;
    }

    public IWindowService ActivateWindow(string key, object? parameter = default)
    {
        // Creates a new scope for resources owned by the window
        IServiceScope scope = _windowProvider.CreateScope();

        // Constructs the window
        Type windowType = RegisteredWindows[key].WindowType; // windowType is guaranteed to be a subclass of TWindowBase when the activation service is built
        TWindowBase window = (TWindowBase?)scope.ServiceProvider.GetService(windowType)
            ?? throw new InvalidOperationException($"E002,{windowType}");

        // If the window supports navigation, initialize the navigation service for the window scope
        // The navigation service may have been instantiated and injected into 'window' already.
        if (window is INavigationProvider navProvider)
        {
            var navService = scope.ServiceProvider.GetRequiredService<INavigationService>();
            navService.InitializeNavigation(navProvider, scope, null);
        }

        // Configures the scope to be disposed when the window is closed
        ConfigureWindowClose(window, scope);

        // Activates the window
        return ActivateWindow(window, parameter);
    }

    /// <summary>
    /// 激活对应的 <paramref name="window"/> 并且返回一个能控制该窗口 <see cref="IWindowService"/> 的对象
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="parameter">传递的参数</param>
    /// <returns></returns>
    protected abstract IWindowService ActivateWindow(TWindowBase window, object? parameter = default);

    /// <summary>
    /// Configure the <paramref name="window"/> to dispose the <paramref name="scope"/> and removes itself from ActiveWindows when it is closed.
    /// </summary>
    /// <param name="window">窗口</param>
    /// <param name="scope"></param>
    protected abstract void ConfigureWindowClose(TWindowBase window, IServiceScope scope);
}
