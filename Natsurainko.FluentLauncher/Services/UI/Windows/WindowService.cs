using Microsoft.UI.Xaml;
using WinUIEx;

namespace Natsurainko.FluentLauncher.Services.UI.Windows;

/// <summary>
/// WinUI 窗口的 <see cref="IActivationService"/> 的默认实现
/// </summary>
internal class WindowService : IWindowService
{
    private readonly Window _window;

    public object? ActivatingParameter { get; init; }

    public Window Window => _window;

    public string Title
    {
        get => _window.Title;
        set => _window.Title = value;
    }

    public WindowService(Window window, object? parameter)
    {
        _window = window;
        ActivatingParameter = parameter;
    }

    public void Close() => _window.Close();

    public void Hide() => _window.Hide();

    public void Activate() => _window.Activate();
}
