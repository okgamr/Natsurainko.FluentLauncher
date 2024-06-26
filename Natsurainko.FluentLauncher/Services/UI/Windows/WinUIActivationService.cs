using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;

namespace Natsurainko.FluentLauncher.Services.UI.Windows;

internal class WinUIActivationService : ActivationService<Window>
{
    // 工厂模式
    public static ActivationServiceBuilder<WinUIActivationService, Window> GetBuilder(IServiceProvider windowProvider)
    {
        return new ActivationServiceBuilder<WinUIActivationService, Window>(windowProvider)
            .WithServiceFactory((r, p) => new WinUIActivationService(r, p));
    }

    private WinUIActivationService(IReadOnlyDictionary<string, WindowDescriptor> registeredWindows, IServiceProvider windowProvier)
        : base(registeredWindows, windowProvier) { }

    protected override IWindowService ActivateWindow(Window window, object? parameter = default)
    {
        _activeWindows.Add(window);
        window.Activate();

        return new WindowService(window, parameter);
    }

    protected override void ConfigureWindowClose(Window window, IServiceScope scope)
    {
        window.Closed += (_, _) =>
        {
            scope.Dispose();
            _activeWindows.Remove(window);
        };
    }

}
