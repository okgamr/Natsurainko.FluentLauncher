using System;
using System.Collections.Generic;

namespace Natsurainko.FluentLauncher.Services.UI.Windows;

/// <summary>
/// 用于激活应用程序窗口的服务
/// 窗口可以使用字符串键注册为单个实例或多个实例
/// </summary>
internal interface IActivationService
{
    /// <summary>
    /// 记录了字符串键到窗口类型的映射的字典
    /// </summary>
    IReadOnlyDictionary<string, WindowDescriptor> RegisteredWindows { get; }

    /// <summary>
    /// 尝试激活已注册的窗口
    /// </summary>
    /// <remarks>
    /// 如果窗口注册为单实例，它将激活现有实例，或者如果没有此类型的实例，则创建一个新实例。
    /// <br/>
    /// 如果窗口被注册为多个实例，它将创建该窗口的新实例，并通过将其置于前台来激活该窗口。
    /// <br/>
    /// 如果窗口没有被注册，会抛出异常。
    /// </remarks>
    /// <param name="key">窗口注册的键值</param>
    /// <param name="parameter">传递的参数</param>
    /// <returns>创建 并/或 激活的窗口的 IWindowService</returns>
    IWindowService ActivateWindow(string key, object? parameter = default);
}


public record WindowDescriptor(Type WindowType, bool MultiInstance = false);
